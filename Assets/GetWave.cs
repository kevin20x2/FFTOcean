using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof(Cpu2Dfft))]
[RequireComponent (typeof(WaterMesh))]
public class GetWave : MonoBehaviour {

    // Use tks for initialization
    public int waveNumber = 1;
    public int MaxWaveNumber = 10;
    public Material InitMat;
    public Material fftMat;
    public Material HeightMat;
    public Material SpectrumMat;
    public Material MixMat;
    public Material WhiteCapMat;

    public RenderTexture WhiteCap;
    public RenderTexture sumHeightping;
    public RenderTexture sumHeightpong;
    public RenderTexture sumSpectrumping;
    public RenderTexture sumSpectrumpong;

    public int[] _Resolution ;
    public int[] ImageResolution ;
    public RenderTexture []H0Texture;
    public RenderTexture []Spe0Texture;
    public RenderTexture []InitTexture;
    public RenderTexture []Height;
    public RenderTexture []Spectrum;
    public RenderTexture []ping;
    public RenderTexture []pong;
    public float [] timeSpeed ;
    public float [] Amp  ;
    public float []Choppiness ;
    public float []WaveLength ;
    public Vector4[] Wind;
    //Cpu2Dfft fft;
  

    
    public MeshRenderer meshRender;
    public bool inited = false;

    public void init()
    {
        if(inited)
        {
            for (int i = 0; i < waveNumber; ++i)
            {
                Destroy(H0Texture[i]);
                Destroy(InitTexture[i]);
                Destroy(Height[i]);
                Destroy(Spectrum[i]);
                Destroy(Spe0Texture[i]);
                Destroy(ping[i]);
                Destroy(pong[i]);
            }

        }
        //if (!inited)
        {
            //  fft = gameObject.GetComponent<Cpu2Dfft>();
            H0Texture = new RenderTexture[waveNumber];
            InitTexture = new RenderTexture[waveNumber];
            Height = new RenderTexture[waveNumber];
            Spectrum = new RenderTexture[waveNumber];
            Spe0Texture = new RenderTexture[waveNumber];
            ping = new RenderTexture[waveNumber];
            pong = new RenderTexture[waveNumber];
            
            for(int i = 0;i<waveNumber;++i)
            {
                H0Texture[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                InitTexture[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                Height[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                Spectrum[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                Spe0Texture[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                ping[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
                pong[i] = new RenderTexture(ImageResolution[i], ImageResolution[i], 0, RenderTextureFormat.ARGBFloat);
            }
        }
        //initTexture
        for (int i = 0; i < waveNumber; ++i)
        {
            InitMat.SetVector("_Wind", Wind[i]);
            InitMat.SetFloat("_Resolution", _Resolution[i]);
            InitMat.SetFloat("_Amplitude", Amp[i]);
            InitMat.SetFloat("_WaveLength", WaveLength[i]);
            Graphics.Blit(null, InitTexture[i], InitMat);
        }
        //fftMat.SetFloat("_Resolution", _Resolution);
            inited = true;
       
    }
    void init_wave()
    {
        _Resolution[0] = ImageResolution[0] = 32;
        Amp[0] = 5;
        Choppiness[0] = 0.4f;
        WaveLength[0] = 32;
        timeSpeed[0] = 0.7f;
        Wind[0] = new Vector4(0.0f, 1.414f, 0, 0);



        _Resolution[1] = ImageResolution[1] = 128;
        Amp[1] = 1; 
        Choppiness[1] = 0.1f;
        WaveLength[1] = 128;
        timeSpeed[1] = 0.5f;
        Wind[1] = new Vector4(1.0f,-1.0f,0,0);

        _Resolution[2] = ImageResolution[2] = 4;
        Amp[2] = 15;
        Choppiness[2] = 0.4f;
        WaveLength[2] = 4.0f;
        timeSpeed[2] = 0.2f;
        Wind[2] = new Vector4(1.414f,0 , 0, 0);


    }
    void Awake () {
        _Resolution = new int[MaxWaveNumber];
        ImageResolution = new int[MaxWaveNumber];
        Amp = new float[MaxWaveNumber];
        Choppiness = new float[MaxWaveNumber];
        WaveLength = new float[MaxWaveNumber];
        timeSpeed = new float[MaxWaveNumber];
        Wind = new Vector4[MaxWaveNumber];

        WaterMesh wm = gameObject.GetComponent<WaterMesh>();
        WhiteCapMat.SetFloat("_Length",wm._step*wm._resolution);
        meshRender = gameObject.GetComponent<MeshRenderer>();
        meshRender.material.SetInt("_WaveNumber", waveNumber);
        WhiteCap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        sumHeightping = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        sumHeightpong = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        sumSpectrumping = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        sumSpectrumpong = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);

        init_wave();
        init();
	}
	private void fftGPU(RenderTexture input,RenderTexture ans,RenderTexture ping,RenderTexture pong)
        //int width ,int height,Texture input,
       // RenderTexture ans, RenderTexture ping,RenderTexture pong)
    {
        //RenderTexture ans = new RenderTexture(width, height,0, RenderTextureFormat.ARGBFloat);
        //RenderTexture ping = new RenderTexture(width,height,0,RenderTextureFormat.ARGBFloat);
        //RenderTexture pong = new RenderTexture(width,height,0,RenderTextureFormat.ARGBFloat);
        //  ans.filterMode = FilterMode.Point;
        // ping.filterMode = FilterMode.Point;
        //pong.filterMode = FilterMode.Point;
        int height = input.height;
        int width = input.width;
        int iterations = Mathf.CeilToInt(Mathf.Log((float)width, 2.0f));
        fftMat.SetFloat("_TransformSize",height);
        for (int it = 0; it < iterations * 2; ++it)
        {
            RenderTexture intx = it % 2 == 0 ? ping : pong;
            RenderTexture outtx = it % 2 == 0 ? pong : ping;

            int Ns = Mathf.CeilToInt(Mathf.Pow(2.0f, 1.0f * (it % iterations) + 1));
            fftMat.SetFloat("_SubTransformSize", Ns);
            if (it < iterations)
            {
                if (it == 0)
                {
                    fftMat.EnableKeyword("_HORIZONTAL");
                    fftMat.DisableKeyword("_VERTICAL");
                    fftMat.SetTexture("_Input", input);
                    Graphics.Blit(null, outtx, fftMat);
                    //    outtx.SetPixel(i, j, fftrow(i, j, width, Ns, input));
                }
                else
                {
                    fftMat.SetTexture("_Input", intx);
                    Graphics.Blit(null, outtx, fftMat);
                    //outtx.SetPixel(i, j, fftrow(i, j, width, Ns, intx));
                }
            }
            else
            {
                if (it == iterations)
                {
                    fftMat.DisableKeyword("_HORIZONTAL");
                    fftMat.EnableKeyword("_VERTICAL");

                }
                fftMat.SetTexture("_Input", intx);
                //outtx.SetPixel(i, j, fftcol(i, j, height, Ns, intx));
                if (it == iterations * 2 - 1)
                {
                    Graphics.Blit(null, ans, fftMat);
                    //   ans.SetPixel(i, j, fftcol(i, j, height, Ns, intx));
                }
                else Graphics.Blit(null, outtx, fftMat);
            }
        }

        //return ans;
    }


    // Update is called once per frame
    void Update () {
        for (int i = 0; i < waveNumber; ++i)
        {
            float current_time = timeSpeed[i] * Time.time;
            HeightMat.SetFloat("_CpuTime", current_time);
            HeightMat.SetFloat("_Resolution", _Resolution[i]);
            HeightMat.SetFloat("_WaveLength",WaveLength[i]);
            HeightMat.SetTexture("_InitTex", InitTexture[i]);


            SpectrumMat.SetFloat("_CpuTime", current_time);
            SpectrumMat.SetFloat("_Resolution", _Resolution[i]);
            SpectrumMat.SetFloat("_WaveLength", WaveLength[i]);
            SpectrumMat.SetFloat("_Choppiness", Choppiness[i]);
            SpectrumMat.SetTexture("_InitTex", InitTexture[i]);
            Graphics.Blit(null, H0Texture[i], HeightMat);
            Graphics.Blit(null, Spe0Texture[i], SpectrumMat);
            fftGPU(H0Texture[i],Height[i],ping[i],pong[i]);
            fftGPU(Spe0Texture[i],Spectrum[i],ping[i],pong[i]);

            if (i == 0)
                MixMat.SetTexture("_Input1",Texture2D.blackTexture);
            else
                MixMat.SetTexture("_Input1", i % 2 == 0 ? sumHeightping : sumHeightpong);
            MixMat.SetTexture("_Input2",Height[i]);
            Graphics.Blit(null, i%2 == 0 ?sumHeightpong:sumHeightping, MixMat);

            if (i == 0)
                MixMat.SetTexture("_Input1",Texture2D.blackTexture);
            else
                MixMat.SetTexture("_Input1", i % 2 == 0 ? sumSpectrumping : sumSpectrumpong);
            MixMat.SetTexture("_Input2",Spectrum[i]);
            Graphics.Blit(null, i%2==0?sumSpectrumpong:sumSpectrumping, MixMat);
        }
        WhiteCapMat.SetTexture("_Displacement", waveNumber % 2 == 0 ? sumSpectrumping : sumSpectrumpong);
        WhiteCapMat.SetTexture("_Bump",waveNumber%2 == 0? sumSpectrumping:sumSpectrumping);
        Graphics.Blit(null, WhiteCap, WhiteCapMat);
        meshRender.material.SetTexture("_WhiteCapInput", WhiteCap);
        meshRender.material.SetTexture("_HeightInput",waveNumber%2 == 0? sumHeightping:sumHeightpong);
        meshRender.material.SetTexture("_SpectrumInput", waveNumber%2==0?sumSpectrumping:sumSpectrumpong);
		
	}
}
