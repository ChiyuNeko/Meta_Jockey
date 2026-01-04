using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(InputData))]
public class HandsParameters : MonoBehaviour
{
    InputDevice LeftControllerDevice;
    InputDevice RightControllerDevice;
    public VelocetyData velocetyData_L;
    public VelocetyData velocetyData_R;
    public float LeftControllerVelocity;
    public float RightControllerVelocity;
    public Vector3 LeftControllerAcceleration;
    public Vector3 RightControllerAcceleration;
    public float Acceleration;
    InputData inputData;

    void Start()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        inputData = GetComponent<InputData>();
    }

    void Update()
    {
        if(velocetyData_L)
            LeftControllerVelocity = velocetyData_L.Velocety;
        if(velocetyData_R)
            RightControllerVelocity = velocetyData_R.Velocety;
        // 取得左右手計算後的速率

        inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerAcceleration);
        inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerAcceleration);
        // 輸出左右手的加速度

        Acceleration = RightControllerAcceleration.magnitude;
        // 輸出右手加速度的大小
    }


}
