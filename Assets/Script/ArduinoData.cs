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
        public int encoder2;
        public int button1;
        public int button2;
        public int button3;
        public int button4;
        public int button5;
        public int button6;
        public int button7;
        void Start()
        {
            ArduinoBasic arduinoBasic = new ArduinoBasic();
        }

        void Update()
        {
            datas = arduinoBasic.GetArduinoParameter();

            if (datas.Length == 10)
            {
                encoder = int.Parse(datas[1]);
                encoder2 = int.Parse(datas[2]);
                button1 = int.Parse(datas[3]);
                button2 = int.Parse(datas[4]);
                button3 = int.Parse(datas[5]);
                button4 = int.Parse(datas[6]);
                button5 = int.Parse(datas[7]);
                button6 = int.Parse(datas[8]);
                button7 = int.Parse(datas[9]);
            }

        }
    }

}
