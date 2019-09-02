using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FalaPredefinida {
    AUDIO_APRESENTACAO,
    AUDIO_MENU_APRESENTACAO,
}

public class AudioPredefinido{
    public FalaPredefinida NOME;
    public AudioClip AUDIO;

    public AudioPredefinido(FalaPredefinida NAME_IN, AudioClip AUDIO_IN){
        this.NOME = NAME_IN;
        this.AUDIO = AUDIO_IN;
    }
    public AudioPredefinido(){
        
    }
}

public class ItemFilaAudio{
    public AudioClip AUDIO;
    public bool podeLibear;

    public ItemFilaAudio(AudioClip AUDIO_IN, bool LIBERA_AUDIO){
        this.podeLibear = LIBERA_AUDIO;
        this.AUDIO = AUDIO_IN;
    }

    public void Termina(){
        if(this.podeLibear) Object.Destroy(this.AUDIO);
    }
}

public class WebTTS : MonoBehaviour
{
    public AudioSource _audio;
    private List<AudioPredefinido> _falasPredef = new List<AudioPredefinido>();
    private List<ItemFilaAudio> _filaAudios = new List<ItemFilaAudio>();

    private int initialAudioDownloaded=0, maxAudioDownloaded=0;

    // Start is called before the first frame update
    void Start()
    {
        _audio  = gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update(){
        if(!this._audio.isPlaying){
            if(_audio.clip != null){
                _audio.clip = null;
                Debug.Log("...Audio encerrado");
                this._filaAudios[0].Termina();
                this._filaAudios.RemoveAt(0);
            }
            if(this._audio.clip == null && (this._filaAudios.Count > 0)){
                Debug.Log("Começando audio...");
                _audio.clip = this._filaAudios[0].AUDIO;
                _audio.Play();
            }
        } Debug.Log("Fila: "+this._filaAudios.Count);
    }

    public void PlayFala(string Fala){
        StartCoroutine(DownloadAndPlayAudio(Fala));
    }

    IEnumerator DownloadAndPlayAudio(string TextIn){
        string LINK = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen="+TextIn.Length+"&client=tw-ob&q="+TextIn+"&tl=Pt-br";
        WWW www = new WWW(LINK);
        yield return www;

        Debug.Log("+1 runtime audio");

        this._filaAudios.Add( new ItemFilaAudio(www.GetAudioClip(false, true, AudioType.MPEG), true) );

    }

    AudioClip DownloadAndGetAudio(string TextIn){
        string LINK = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen="+TextIn.Length+"&client=tw-ob&q="+TextIn+"&tl=Pt-br";
        WWW www = new WWW(LINK);
        return www.GetAudioClip(false, true, AudioType.MPEG);
    }

    IEnumerator addAudioClipByString(string TEXT, FalaPredefinida FALA_ID){
        this.maxAudioDownloaded++;
        this._falasPredef.Add( new AudioPredefinido( FALA_ID, DownloadAndGetAudio(TEXT) )  );
        this.initialAudioDownloaded++;
        yield return true;
    }

    public void PrepareBasicAudios(){
        StartCoroutine( addAudioClipByString(
            "Olá! Seja bem-vindo ou bem-vinda. Vamos começar... Precisamos de algumas permissões do seu celular.",
            FalaPredefinida.AUDIO_APRESENTACAO
            )
        );
        StartCoroutine( addAudioClipByString(
            "Para começar um comando de voz, incline a cabeça para a sua esquerda.",
            FalaPredefinida.AUDIO_MENU_APRESENTACAO
            )
        );
        while(this.maxAudioDownloaded == 0 || ( this.maxAudioDownloaded-this.initialAudioDownloaded > 0 ) ){ 
            //Espera
        }
    }

    public bool PlayAudioPredefinido(FalaPredefinida ESCOLHA){
        foreach (var PREDEF in this._falasPredef){
            if(ESCOLHA == PREDEF.NOME){
                this._filaAudios.Add( new ItemFilaAudio(PREDEF.AUDIO, false) );
                return true;
            }
        }
        return false;
    }
}
