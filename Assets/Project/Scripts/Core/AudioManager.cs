using UnityEngine;
using UnityEngine.Audio;

namespace WhaleShark.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;

        [Header("Mixer (Expose BGMVol, SEVol)")]
        public AudioMixer mixer;

        [Header("SE Pool Size")]
        public int seSources = 8;

        AudioSource bgmA, bgmB;
        bool bgmAActive = true;
        AudioSource[] sePool;
        int seIndex;

        void Awake()
        {
            if(I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
            DontDestroyOnLoad(gameObject);

            bgmA = gameObject.AddComponent<AudioSource>();
            bgmA.loop = true;
            bgmB = gameObject.AddComponent<AudioSource>();
            bgmB.loop = true;

            // AudioMixer 그룹 설정 (Mixer가 설정된 경우)
            if (mixer != null)
            {
                var bgmGroup = mixer.FindMatchingGroups("BGM");
                var seGroup = mixer.FindMatchingGroups("SE");

                if (bgmGroup.Length > 0)
                {
                    bgmA.outputAudioMixerGroup = bgmGroup[0];
                    bgmB.outputAudioMixerGroup = bgmGroup[0];
                }

                if (seGroup.Length > 0)
                {
                    sePool = new AudioSource[seSources];
                    for(int i = 0; i < sePool.Length; i++)
                    {
                        sePool[i] = gameObject.AddComponent<AudioSource>();
                        sePool[i].outputAudioMixerGroup = seGroup[0];
                    }
                }
            }
            else
            {
                sePool = new AudioSource[seSources];
                for(int i = 0; i < sePool.Length; i++)
                {
                    sePool[i] = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        public static void PlayBGM(AudioClip clip, float fade = 0.4f)
            => I.StartCoroutine(I.CrossfadeBGM(clip, fade));

        System.Collections.IEnumerator CrossfadeBGM(AudioClip next, float fade)
        {
            var srcIn = bgmAActive ? bgmB : bgmA;
            var srcOut = bgmAActive ? bgmA : bgmB;
            bgmAActive = !bgmAActive;

            srcIn.clip = next;
            srcIn.volume = 0f;
            srcIn.Play();

            float t = 0f;
            while(t < fade)
            {
                t += Time.unscaledDeltaTime;
                float k = t / fade;
                srcIn.volume = k;
                srcOut.volume = 1f - k;
                yield return null;
            }

            srcIn.volume = 1f;
            srcOut.Stop();
            srcOut.volume = 1f;
        }

        public static void PlaySE(AudioClip clip, float vol = 1f)
        {
            if (I.sePool == null || clip == null) return;
            var a = I.sePool[I.seIndex++ % I.sePool.Length];
            a.PlayOneShot(clip, vol);
        }

        public static void SetBGMVolume(float v01) => SetVol("BGMVol", v01);
        public static void SetSEVolume(float v01) => SetVol("SEVol", v01);

        static void SetVol(string exposedParam, float v01)
        {
            v01 = Mathf.Clamp01(v01);
            float db = Mathf.Log10(Mathf.Max(0.0001f, v01)) * 20f;
            if (I.mixer) I.mixer.SetFloat(exposedParam, db);
        }
    }
}