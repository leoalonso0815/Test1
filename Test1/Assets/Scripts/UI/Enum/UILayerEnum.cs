/*=================================================
*FileName:     UILayerEnum.cs 
*Author:       Z_Sprite 
*UnityVersion：2018.2.12f1 
*Date:         2018-11-03 17:59 
*Description:
*History: 
=================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UILayerEnum
{
    Base,
    Panel,
    Window,
    Chat,
    Tips,
    Top,
    ClickEffect,//屏幕上点击特效的层级 应为最高 需要加层级至于其之下
}
