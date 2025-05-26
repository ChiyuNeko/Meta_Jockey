using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arduino
{
    public class ArduinoData : MonoBehaviour
    {
        public ArduinoBasic arduinoBasic;
        public string[] datas;
        public int encoder;
        public int button1;
        public int button2;
        public int button3;
        void Start()
        {
            ArduinoBasic arduinoBasic = new ArduinoBasic();
        }

        void Update()
        {
            datas = arduinoBasic.GetArduinoParameter();
            encoder = int.Parse(datas[0]);
            button1 = int.Parse(datas[1]);
            button2 = int.Parse(datas[2]);
            button3 = int.Parse(datas[3]);

        }
    }

}
