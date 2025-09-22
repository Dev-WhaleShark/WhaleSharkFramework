using UnityEngine;
using UnityEngine.Audio;

namespace WhaleShark.Core
{
    /// <summary>
    /// 오디오 관리 매니저
    /// BGM 크로스페이드, SE 다중 재생, 랜덤 볼륨 제어 등을 제공
    /// 싱글톤 패턴으로 구현되어 어디서나 접근 가능하며 DontDestroyOnLoad로 설정됨
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>싱글톤 인스턴스</summary>
        public static AudioManager I;

        [Header("Mixer (Expose BGMVol, SEVol)")]
        public AudioMixer mixer;

        [Header("SE Pool Size")]
        public int seSources = 8;

        // 그룹/파라미터 명 상수화
        const string GroupBgm = "BGM";
        const string GroupSe = "SE";
        const string ParamBgmVol = "BGMVol";
        const string ParamSeVol = "SEVol";

        AudioSource bgmA, bgmB;
        bool bgmAActive = true;
        AudioSource[] sePool;
        int seIndex;

        // Mixer 미사용 시에도 전역 볼륨 제어를 위한 내부 마스터
        float bgmMaster = 1f;
        float seMaster = 1f;

        void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
            DontDestroyOnLoad(gameObject);

            seSources = Mathf.Max(1, seSources);

            // BGM 소스 생성
            bgmA = gameObject.AddComponent<AudioSource>();
            bgmA.loop = true;
            bgmB = gameObject.AddComponent<AudioSource>();
            bgmB.loop = true;

            // Mixer 그룹 탐색 (있을 때만 적용)
            AudioMixerGroup bgmGroup = null;
            AudioMixerGroup seGroup = null;
            if (mixer != null)
            {
                var bgmGroups = mixer.FindMatchingGroups(GroupBgm);
                if (bgmGroups != null && bgmGroups.Length > 0) bgmGroup = bgmGroups[0];
                var seGroups = mixer.FindMatchingGroups(GroupSe);
                if (seGroups != null && seGroups.Length > 0) seGroup = seGroups[0];
            }

            if (bgmGroup != null)
            {
                bgmA.outputAudioMixerGroup = bgmGroup;
                bgmB.outputAudioMixerGroup = bgmGroup;
            }

            // SE 풀 생성 (항상 생성, 그룹이 있으면 할당)
            sePool = new AudioSource[seSources];
            for (int i = 0; i < sePool.Length; i++)
            {
                sePool[i] = gameObject.AddComponent<AudioSource>();
                if (seGroup != null) sePool[i].outputAudioMixerGroup = seGroup;
            }
        }

        public static void PlayBGM(AudioClip clip, float fade = 0.4f)
        {
            if (I == null) return;
            if (clip == null)
            {
                // null 클립이면 전부 정지
                I.bgmA.Stop(); I.bgmB.Stop();
                I.bgmA.volume = 0f; I.bgmB.volume = 0f;
                return;
            }

            if (fade <= 0f)
            {
                // 즉시 전환
                var srcIn = I.bgmAActive ? I.bgmB : I.bgmA;
                var srcOut = I.bgmAActive ? I.bgmA : I.bgmB;
                I.bgmAActive = !I.bgmAActive;

                srcOut.Stop();
                srcIn.clip = clip;
                srcIn.volume = I.mixer ? 1f : I.bgmMaster; // Mixer가 있으면 소스 볼륨은 1 유지
                srcIn.Play();
                return;
            }

            I.StartCoroutine(I.CrossfadeBGM(clip, fade));
        }

        System.Collections.IEnumerator CrossfadeBGM(AudioClip next, float fade)
        {
            var srcIn = bgmAActive ? bgmB : bgmA;
            var srcOut = bgmAActive ? bgmA : bgmB;
            bgmAActive = !bgmAActive;

            srcIn.clip = next;
            srcIn.volume = 0f;
            srcIn.Play();

            float t = 0f;
            while (t < fade)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / fade);
                if (mixer)
                {
                    // Mixer 사용 시 소스 볼륨은 crossfade만 담당 (절대값 0..1)
                    srcIn.volume = k;
                    srcOut.volume = 1f - k;
                }
                else
                {
                    // Mixer 미사용 시 내부 마스터 반영
                    srcIn.volume = k * bgmMaster;
                    srcOut.volume = (1f - k) * bgmMaster;
                }
                yield return null;
            }

            if (mixer)
            {
                srcIn.volume = 1f;
                srcOut.Stop();
                srcOut.volume = 1f;
            }
            else
            {
                srcIn.volume = 1f * bgmMaster;
                srcOut.Stop();
                srcOut.volume = 1f * bgmMaster;
            }
        }

        public static void PlaySe(AudioClip clip, float vol = 1f)
        {
            if (I == null || I.sePool == null || clip == null) return;
            var a = I.sePool[I.seIndex++ % I.sePool.Length];
            a.PlayOneShot(clip, Mathf.Clamp01(vol) * (I.mixer ? 1f : I.seMaster));
        }

        public static void SetBgmVolume(float v01) => SetVol(ParamBgmVol, Mathf.Clamp01(v01));
        public static void SetSeVolume(float v01) => SetVol(ParamSeVol, Mathf.Clamp01(v01));

        static void SetVol(string exposedParam, float v01)
        {
            if (I == null) return;

            if (I.mixer)
            {
                // Mixer가 있으면 Mixer 파라미터로만 제어, 내부 마스터는 1 유지
                float db = Mathf.Log10(Mathf.Max(0.0001f, v01)) * 20f;
                I.mixer.SetFloat(exposedParam, db);
                if (exposedParam == ParamBgmVol) I.bgmMaster = 1f; else if (exposedParam == ParamSeVol) I.seMaster = 1f;
            }
            else
            {
                // Mixer가 없으면 내부 마스터를 사용하고 즉시 반영
                if (exposedParam == ParamBgmVol)
                {
                    I.bgmMaster = v01;
                    I.ApplyBGMVolume();
                }
                else if (exposedParam == ParamSeVol)
                {
                    I.seMaster = v01;
                    // 현재 구조에서는 원샷 기준이라 즉시 반영 필요 없음
                }
            }
        }

        // 현재 크로스페이드 비율을 보존하며 마스터 변경 반영
        void ApplyBGMVolume()
        {
            float sum = bgmA.volume + bgmB.volume;
            if (sum > 0f)
            {
                float aRatio = bgmA.volume / sum;
                float bRatio = bgmB.volume / sum;
                bgmA.volume = aRatio * bgmMaster;
                bgmB.volume = bRatio * bgmMaster;
            }
            else
            {
                // 정지 상태이거나 볼륨 0이면 활성 소스를 기준으로 설정
                var active = bgmAActive ? bgmA : bgmB;
                var inactive = bgmAActive ? bgmB : bgmA;
                active.volume = 1f * bgmMaster;
                inactive.volume = 0f;
            }
        }

        // 선택: 외부에서 BGM 정지 필요 시 사용
        public static void StopBGM()
        {
            if (I == null) return;
            I.bgmA.Stop();
            I.bgmB.Stop();
            I.bgmA.volume = 0f;
            I.bgmB.volume = 0f;
        }
    }
}