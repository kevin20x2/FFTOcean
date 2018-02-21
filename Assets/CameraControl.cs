using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Camera))]
public class CameraControl : MonoBehaviour {
    public float step = 20f;
    public float top = 2000f;
    public float bottom = 20f;
    public float forward = -400f;
    public float backward = 4000f;
    private GUILayoutOption [] button_option;
    public float button_height = 60;
    public float button_width = 150;
    private bool show_flag = false;
    private bool show_fft = false;
    bool show_height = false;
    bool show_spectrum = false;
    bool show_mesh_wire = false;
    bool inited = false;
    GameObject water;
    GetWave wave;
    GUIStyle button_style;

    //

    int show_number = -1;
    int wavenumber;
    int [] resolution;
    float [] Amp;
    float [] Choppiness;
    float  []speed;
    float [] WaveLength;

	// Use this for initialization
    void init()
    {
        wavenumber = wave.waveNumber;
        resolution = new int[wave.MaxWaveNumber];
        Amp = new float[wave.MaxWaveNumber];
        Choppiness = new float[wave.MaxWaveNumber];
        speed = new float[wave.MaxWaveNumber];
        WaveLength = new float[wave.MaxWaveNumber];

       // while (!wave.inited) ;

       

    }
    void Start () {

        water = GameObject.Find("Water");
        wave = water.GetComponent<GetWave>();
        List <GUILayoutOption> option = new List<GUILayoutOption>();
        option.Add(GUILayout.MinHeight(button_height));
        option.Add(GUILayout.MinWidth(button_width));
//        option.Add(GUILayout.)
        button_option = option.ToArray();
        button_style = new GUIStyle("box");
        button_style.fontSize = (int)button_height - 20;
        button_style.normal.textColor = Color.white;
        init();
        //button_style.normal.background = ;
		
	}
    private void OnGUI()
    {

        GUILayout.BeginArea(new Rect(100, Screen.height - button_height * 2 - 60, button_width * 5 + 30, button_height * 2 + 50));
        GUILayout.BeginHorizontal("box");

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("up", button_style, button_option))
        {
            if(gameObject.transform.position.y < top)
            gameObject.transform.position += new Vector3(0, step, 0);
        }
        if (GUILayout.Button("down", button_style, button_option))
        {
            if(gameObject.transform.position.y > bottom)
            gameObject.transform.position += new Vector3(0, -step, 0);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("forward", button_style, button_option))
        {
            if(gameObject.transform.position.z >forward)
            gameObject.transform.position += new Vector3(0, 0, -step);
        }
        if (GUILayout.Button("back", button_style, button_option))
        {
            if(gameObject.transform.position.z < backward)
            gameObject.transform.position += new Vector3(0, 0, step);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width-180,100,button_width,button_height *7));

        GUILayout.BeginVertical("box");
        for (int i = 0; i < wavenumber; ++i)
        {
            if (GUILayout.Button(string.Format("Wave {0:D}",i), button_style,button_option))
            {
                //show_number = i;
                if (i == show_number)
                    show_flag = !show_flag;
                else
                {
                    show_number = i;
                    show_flag = true;
                }
                //Debug.Log(show_flag);
            }
        }
        if(GUILayout.Button("Height",button_style,button_option))
        {
            show_height = !show_height;
        }
        if(GUILayout.Button("Spectrum",button_style,button_option))
        {
            show_spectrum = !show_spectrum;
        }
        if(GUILayout.Button("Wire",button_style,button_option))
        {
            show_mesh_wire = !show_mesh_wire;
        }
       

        GUILayout.EndVertical();
        GUILayout.EndArea();
        if (show_height)
        {
            //Debug.Log(show_fft);
            GUI.DrawTexture(new Rect(Screen.width-200, Screen.height - 200, 200, 200), wavenumber % 2 == 0 ? wave.sumHeightping : wave.sumHeightpong, ScaleMode.StretchToFill, false);
        }
        if(show_spectrum)
        {
            GUI.DrawTexture(new Rect(Screen.width-450, Screen.height - 200, 200, 200), wavenumber % 2 == 0 ? wave.sumSpectrumping : wave.sumSpectrumpong, ScaleMode.StretchToFill, false);
        }
        if(show_mesh_wire)
        {
            //wave.meshRender.material = new Material(Shader.Find(""));
            wave.meshRender.material.SetInt("_ShowWire", 1);
        }
        else
        {
            wave.meshRender.material.SetInt("_ShowWire", 0);
        }

            //Debug.Log(show_flag);
        if(show_flag)
        {
            GUILayout.BeginArea(new Rect(Screen.width/2 - 200,100,button_width* 5 +100,button_height*7+100));
            GUILayout.BeginVertical("box");

            GUILayout.Label(string.Format("Wave {0:D}",show_number),button_style,button_option);

            // resolution config
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Resolution", button_style, button_option);
            GUILayout.Label(string.Format("{0:D}",(int)resolution[show_number]), button_style, button_option);
            if(GUILayout.Button("+", button_style, button_option))
            {
                if(resolution[show_number] < 256)
                    resolution[show_number] *= 2;
            }
            if (GUILayout.Button("-", button_style, button_option))
            {
                if (resolution[show_number] > 1)
                    resolution[show_number] /=2;
            }
            GUILayout.EndHorizontal();
            // wavelength config
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("WaveLength", button_style, button_option);
            GUILayout.Label(string.Format("{0:D}",(int)WaveLength[show_number]), button_style, button_option);
            if(GUILayout.Button("+", button_style, button_option))
            {
                if(WaveLength[show_number] < 256)
                    WaveLength[show_number] += 2 ;
            }
            if (GUILayout.Button("-", button_style, button_option))
            {
                if (WaveLength[show_number] > 1)
                    WaveLength[show_number] -=2;
            }
            GUILayout.EndHorizontal();

            // ampitude config

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Ampitude", button_style, button_option);
            GUILayout.Label(string.Format("{0:N}",(int)Amp[show_number]), button_style, button_option);
            if(GUILayout.Button("+", button_style, button_option))
            {
                // if(resolution < 64)
                Amp[show_number] += 2;
            }
            if (GUILayout.Button("-", button_style, button_option))
            {
                //if (resolution > 8)
                Amp[show_number] -= 2;
                Amp[show_number] = Mathf.Max(0.1f,Amp[show_number]);
            }
            if (GUILayout.Button("Min", button_style, button_option))
            {
                //if (resolution > 8)
                Amp[show_number] = 1;
            }

            GUILayout.EndHorizontal();

            //Choppiness config
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Choppiness", button_style, button_option);
            GUILayout.Label(string.Format("{0:N}",Choppiness[show_number]), button_style, button_option);
            if(GUILayout.Button("+", button_style, button_option))
            {
                // if(resolution < 64)
                Choppiness[show_number] += 0.1f;
            }
            if (GUILayout.Button("-", button_style, button_option))
            {
                //if (resolution > 8)
                if (Choppiness[show_number] > 0.1f)
                    Choppiness[show_number] -= 0.1f;
            }
            GUILayout.EndHorizontal();
            //speed 
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("speed", button_style, button_option);
            GUILayout.Label(string.Format("{0:N}",speed[show_number]), button_style, button_option);
            if(GUILayout.Button("+", button_style, button_option))
            {
                // if(resolution < 64)
                speed[show_number] += 0.1f;
                //spped = Mathf
            }
            if (GUILayout.Button("-", button_style, button_option))
            {
                //if (resolution > 8)
                speed[show_number] -= 0.1f;
                speed[show_number] = Mathf.Max(0.1f,speed[show_number]);

            }
            GUILayout.EndHorizontal();


            if(GUILayout.Button("Confirm", button_style, button_option))
            {
                wave._Resolution[show_number] = resolution[show_number];
                wave.ImageResolution[show_number] = resolution[show_number];
                wave.Choppiness[show_number] = Choppiness[show_number];

                wave.Amp[show_number] = Amp[show_number];
                wave.timeSpeed[show_number] = speed[show_number];
                wave.WaveLength[show_number] = WaveLength[show_number];

                wave.init();

            }

            GUILayout.EndVertical();


            GUILayout.EndArea();

        }
        //GUI.Button(new Rect(Screen.width-100,200,60,80), "reset");

    }
    // Update is called once per frame
    void Update () {
        if (wave.inited && !inited)
        {
            wave._Resolution.CopyTo(resolution, 0);
            wave.Amp.CopyTo(Amp, 0);
            wave.Choppiness.CopyTo(Choppiness, 0);
            wave.timeSpeed.CopyTo(speed, 0);
            wave.WaveLength.CopyTo(WaveLength, 0);
            inited = true;
        }

    }
}
