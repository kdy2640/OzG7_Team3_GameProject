using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//프로젝트 전체에서 하나만 존재하도록 관리된다.
public enum BGMType
{
    None,
    TitieBGM = 0,
    HubBGM = 1,
    ServiceBGM = 2,
    HarvestBGM = 3,
}
public enum SFXType
{
    None,

    //Global
    Global_ButtonClick = 0,
    Global_ButtonHover = 1,
    Global_SceneChange = 2,
    Global_Notification = 3,
    Global_Error = 4,
    //Hub
    Hub_Upgrade = 100,
    Hub_FacilitySelect = 101,
    Hub_StaffSelect = 102,
    Hub_MenuSelect = 103,
    Hub_MenuDeselect = 104,
    Hub_Recruit = 105,
    Hub_ServiceStart = 106,
    Hub_LevelUp = 107,
    Hub_GetReward = 108,
    Hub_Rankup = 109,
    Hup_PanelPopup = 110,
    //Service
    Service_SessionStart = 200,
    //Harvest
    Harvest_SessionStart = 300,
    Harvest_Collect = 301,
    Harvest_Grind = 302,
    Harvest_CropHarvested = 303,
    Harvest_TractorEngine = 304,
}
public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private int sfxSourceCount = 20;//10에서 20으로 변경
    [SerializeField] private AudioSource[] sfxSources;

    // AudioSource를 순환 관리하기 위한 Queue
    private Queue<AudioSource> sfxQueue;

    [Header("BGM List")]
    [SerializeField] private BGMClipData[] bgmClips;//인스펙터에서 등록할 BGM
    [Header("SFX List")]
    [SerializeField] private SFXClipData[] sfxClips;//인스펙터에서 등록할 효과음 데이터


    //예 : BGMType.Stage -> Stage BGM데이터
    private Dictionary<BGMType, BGMClipData> bgmDictionary;
    //예 : SFXType.Jump -> Jump 효과음 데이터
    private Dictionary<SFXType, SFXClipData> sfxDictionary;

    // 각 효과음이 마지막으로 재생된 시간을 저장하는 딕셔너리
    // Key   : 효과음 종류(SFXType)
    // Value : 마지막 재생 시간(Time.time)
    private Dictionary<SFXType, float> lastPlayTimes;
    private Dictionary<SFXType, int> playingCounts;

    private BGMClipData currentBGMData;
    private float masterVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private float sfxVolume = 1.0f;

    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    protected void Awake()
    {  
        sfxQueue = new Queue<AudioSource>();

        CreateAudioSources();
        // 각 효과음의 마지막 재생 시간을 저장하는 Dictionary 생성
        lastPlayTimes = new Dictionary<SFXType, float>();
        playingCounts = new Dictionary<SFXType, int>();

        InitializeDictionary();
    }
   //AudioSource가 없을경우 자동으로 만들어주는 녀석
   private void CreateAudioSources()
    {
        if(bgmSource==null)
        {
            //BGM source 라는 이름의 빈 게임오브젝트를 생성하자.
            GameObject bgmObj = new GameObject("BGM source");
            bgmObj.transform.SetParent(transform);

            //생성한 오브젝트에 AudioSource컴포넌트를 추가
            bgmSource = bgmObj.AddComponent<AudioSource>();

            //BGM은 반복재생하니까 루프를 true로 설정
            bgmSource.loop = true;
        }
        if (sfxSources == null || sfxSources.Length == 0)
        {
            sfxSources = new AudioSource[sfxSourceCount];

            for (int i = 0; i < sfxSourceCount; i++)
            {
                GameObject sfxObj = new GameObject($"SFX Source {i}");
                sfxObj.transform.SetParent(transform);

                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.loop = false;

                sfxSources[i] = source;

                sfxQueue.Enqueue(source);
            }
        }
    }
    private AudioSource GetSFXSource()
    {
        int count = sfxQueue.Count;

        // Queue에 있는 AudioSource를 모두 검사
        for (int i = 0; i < count; i++)
        {
            AudioSource source = sfxQueue.Dequeue();

            // 다시 Queue의 뒤에 넣는다.
            sfxQueue.Enqueue(source);

            // 사용 가능하면 즉시 반환
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null; //오디오소스가 10개인 경우 새 SFX효과음은 무시하고 기존 10개만 끝까지 재생
    }
    //배열로 등록한 오디 데이터를 딕셔너리에 저장하는 녀석
    private void InitializeDictionary()
    {
        bgmDictionary = new Dictionary<BGMType, BGMClipData>();
        sfxDictionary = new Dictionary<SFXType, SFXClipData>();

        for (int i = 0;i<bgmClips.Length;i++)
        {
            //배열 요소가 비어 있으면
            if (bgmClips[i] == null) continue;
            //BGM데이터 안에 AudioClip이 연결되어 있지 않으면
            if (bgmClips[i].clip == null) continue;

            //딕셔너리에 같은 BGMType이 아직 없으면
            if (!bgmDictionary.ContainsKey(bgmClips[i].type))
            {
                //BGMType을 key, BGMClipData를 Value로 저장
                bgmDictionary.Add(bgmClips[i].type, bgmClips[i]);
            }
        }
        for(int i = 0;i<sfxClips.Length;i++)
        {
            if (sfxClips[i] == null) continue;
            if (sfxClips[i].clip == null) continue;

            //딕셔너리에 같은 SFXType이 아직 없으면
            if (!sfxDictionary.ContainsKey(sfxClips[i].type))
            {
                sfxDictionary.Add(sfxClips[i].type, sfxClips[i]);
            }
        }
    }
    //BGM을 재생하는 녀석
    //AudioManager.Instance.PlayBGM(BGMType.Stage);
    public void PlayBGM(BGMType type)
    {
        //요청한 BGMType이 딕셔너리에 없으면
        if(!bgmDictionary.ContainsKey(type))
        {
            return;
        }
        //딕셔너리에서 해당 BGM데이터를 가져온다.
        BGMClipData data = bgmDictionary[type];

        //현재 재생중인 BGM과 요청한 BGM이 같다면
        if(bgmSource.clip==data.clip)
        {
            return;
        }
        //현재 재생중인 BGM데이터를 저장
        currentBGMData = data;
        //BGM AudioSource에 재생할 AudioClip을 넣는다.
        bgmSource.clip = data.clip;

        bgmSource.volume = data.volume * bgmVolume * masterVolume;
        //BGM을 재생한다.
        bgmSource.Play();
    }
    //BGM을 정지시키는 퓬
    public void StopBGM()
    {
        //현재 재쇼ㅐㅇ중인 BGM을 정지
        bgmSource.Stop();
        //오디오 소스에 연결된 오디오 클립을 제거
        bgmSource.clip = null;
        //현재 재생중인 BGM데이터도 비우자.
        currentBGMData = null;
    }
    //일시 정지
    public void PauseBGM()
    {
        bgmSource.Pause();
    }
    //일시정지된 BGM을 다시 재생
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    //효과음 랜덤으로 Randomratio (0~0.2) 추천
    public void PlaySFXRandomPitch(SFXType type, float randomRatio)
    {
        if (!sfxDictionary.ContainsKey(type))
            return;

        SFXClipData data = sfxDictionary[type];
        // 현재 같은 효과음이 최대 개수 이상 재생 중이면 재생하지 않는다.
        if (playingCounts.TryGetValue(type, out int count))
        {
            if (count >= data.maxSimultaneousCount) return;
        }
        AudioSource source = GetSFXSource();
        if (source == null) return;

        float volume = data.volume * sfxVolume * masterVolume;
        // 현재 재생 중인 개수 증가
        if (!playingCounts.ContainsKey(type))
        {
            playingCounts[type] = 0;
        }
        playingCounts[type]++;

        source.pitch = data.pitch + Random.Range(-randomRatio, randomRatio);

        source.PlayOneShot(data.clip, volume);
        // 효과음이 끝나면 재생 중 개수를 감소시킨다.
        StartCoroutine(ReleaseVoice(type, data.clip.length));
    }
    //효과음 재생하는 녀석
    public void PlaySFX(SFXType type)
    {
        if (!sfxDictionary.ContainsKey(type)) return;

        SFXClipData data = sfxDictionary[type];
        //최대 개수를 넘으면 재생X
        if (playingCounts.TryGetValue(type, out int count))
        {
            if (count >= data.maxSimultaneousCount)
                return;
        }
        // 마지막 재생 시간이 저장되어 있다면
        if (lastPlayTimes.TryGetValue(type, out float lastPlayTime))
        {
            // 마지막 재생 후 아직 최소 재생 간격이 지나지 않았다면
            // 이번 재생은 무시한다.
            if (Time.time - lastPlayTime < data.minInterval) return;
        }
        // 이번 재생 시간을 저장한다.
        lastPlayTimes[type] = Time.time;

        AudioSource source = GetSFXSource();
        if (source == null) return;
        
        float volume = data.volume * sfxVolume * masterVolume;

        float pitch = data.pitch;
        //재생직전 개수 증가
        if (!playingCounts.ContainsKey(type))
        {
            playingCounts[type] = 0;
        }
        playingCounts[type]++;

        source.pitch = pitch;
        source.PlayOneShot(data.clip, volume);

        StartCoroutine(ReleaseVoice(type, data.clip.length));
    }
    //효과음 종류 후 카운트 감소용 코루틴
    private IEnumerator ReleaseVoice(SFXType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playingCounts.ContainsKey(type))
        {
            playingCounts[type]--;

            if (playingCounts[type] < 0)
                playingCounts[type] = 0;
        }
    }

    //전체 볼륨을 변경하는 녀석
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }
    //BGM볼륨을 변경하는 녀석
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public AudioSaveData CreateAudioSaveData()
    {
        return new AudioSaveData(masterVolume, bgmVolume, sfxVolume);
    }

    public void LoadAudioSaveData(AudioSaveData saveData)
    {
        if (saveData == null)
        {
            ResetAudioSaveData();
            return;
        }

        SetMasterVolume(saveData.masterVolume);
        SetBGMVolume(saveData.bgmVolume);
        SetSFXVolume(saveData.sfxVolume);
    }

    public void ResetAudioSaveData()
    {
        SetMasterVolume(1f);
        SetBGMVolume(1f);
        SetSFXVolume(1f);
    }

    //현재 재생중인 BGM의 볼륨을 계산
    private void UpdateBGMVolume()
    {
        //bgmSource가 없으면
        if (bgmSource == null) return;
        //현재 재생중인 BGM데이터가 없다면
        if(currentBGMData==null)
        {
            //기본 BGM볼륨과 마스터 볼륨만 적용
            bgmSource.volume = bgmVolume * masterVolume;
            return;
        }
        bgmSource.volume = currentBGMData.volume * bgmVolume * masterVolume;
    }
}
