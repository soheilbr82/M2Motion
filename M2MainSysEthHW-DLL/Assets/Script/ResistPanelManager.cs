﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using DLMotion;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ResistPanelManager : MonoBehaviour {

    public MainSysPanel BckMainPanel;
    public ResistPanelManager ResistSelfPanel;

    public GameObject Player;
    public GameObject Beacon;

    public Button ReturnMotionBtn;
    public Button RelaxMotionBtn;
    public Button StopMotionBtn;
    public Button ReturnMain;
        
    public InputField ResistTorValue;
    public InputField OriValueX;
    public InputField DstValueX;
    public InputField OriValueY;
    public InputField DstValueY;

    public static int UI_ResistVal;
    public static int OriX;
    public static int OriY;
    public static int DstX;
    public static int DstY;

    private bool flag = false;
    public InputField outPath;
    static List<string> mWriteTxt = new List<string>();
    public GameObject redpoint;
    private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  
    private float m_UpdateShowDeltaTime = 0.01f;//更新帧率的时间间隔;  
    private int m_FrameUpdate = 0;//帧数;  
    private float m_FPS = 0;

    void Awake()
    {
        Application.targetFrameRate = 100;
    }

    // Use this for initialization
    void Start ()
    {
        ReturnMain.onClick.AddListener(ReturnMainBtnClick);
        ReturnMotionBtn.onClick.AddListener(ReturnMotionBtnClick);
        RelaxMotionBtn.onClick.AddListener(RelaxMotionBtnClick);
        StopMotionBtn.onClick.AddListener(StopMotionBtnClick);
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }
        if (flag)
        {
            string[] temp = {DynaLinkHS.StatusMotRT.PosDataJ1.ToString(), ",", DynaLinkHS.StatusMotRT.PosDataJ2.ToString(), ",", DynaLinkHS.StatusMotRT.SpdDataJ1.ToString(), ",",
                DynaLinkHS.StatusMotRT.SpdDataJ2.ToString(), ",", DynaLinkHS.StatusMotRT.TorDataJ1.ToString(),",",DynaLinkHS.StatusMotRT.TorDataJ2.ToString(), "\r\n" };
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(outPath.text, true, Encoding.UTF8))
                {
                    writer.Write(t);
                }
                mWriteTxt.Remove(t);
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, 0, 100, 100), "FPS: " + m_FPS);
        GUI.skin.label.normal.textColor = Color.black;
    }

    void ReturnMainBtnClick()
    {
        BckMainPanel.gameObject.SetActive(true);
        ResistSelfPanel.gameObject.SetActive(false);
        Player.SetActive(false);
        Beacon.SetActive(false);
        redpoint.SetActive(false);
    }

    void EMStopBtnClick()
    {
        DynaLinkHS.CmdServoOff();
    }

    void ReturnMotionBtnClick()
    {
        float xPos;
        float yPos;
        Beacon.SetActive(true);
        OriX = int.Parse(OriValueX.text);
        OriY = int.Parse(OriValueY.text);
        xPos = OriX / ModulePara.TrgXPosScale - 900;
        yPos = OriY / ModulePara.TrgYPosScale - 270;
        Beacon.transform.localPosition = new Vector3(xPos, yPos, 0);

        float RedX;
        float RedY;
        redpoint.SetActive(true);
        DstX = int.Parse(DstValueX.text);
        DstY = int.Parse(DstValueY.text);
        RedX = DstX / ModulePara.TrgXPosScale - 900;
        RedY = DstY / ModulePara.TrgYPosScale - 270;
        redpoint.transform.localPosition = new Vector3(RedX, RedY, 0);

        DynaLinkHS.CmdLinePassive(OriX, OriY, 200000);
        flag = false;
    }
    void StopMotionBtnClick()
    {
        DynaLinkHS.CmdServoOff();
        flag = false;
    }
    void RelaxMotionBtnClick()
    {
        UI_ResistVal = int.Parse(ResistTorValue.text);
        DynaLinkHS.CmdResistLT(UI_ResistVal);
        flag = true;
    }
}
