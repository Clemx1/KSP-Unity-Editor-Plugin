﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticLimbIK : RoboticLimb
{
    struct IKAxis { public List<RoboticServoIK> servoGroup; public RoboticServoIK fixedAngleServo; public RoboticServoIK servo0; public RoboticServoIK servo1; }
    IKAxis IKAxisX, IKAxisY, IKAxisZ;
    public List<RoboticServoIK> servosIK;//, servoGroup0, servoGroup1, servoGroup2;
    public Transform limbTarget;
    Transform gait;
    bool IKactive = false;

    public override void CustomStart(LimbController limbController)
    {
        base.CustomStart(limbController);
    }

    public void RunIK()
    {
        CalculateIK(IKAxisX);
        CalculateIK(IKAxisY);
        //   CalculateIK(IKAxisZ);
    }
    public Transform testTarget;
    public int limbDistCheckCount = 0;
    float servo1_0Dist;
    float servo0_GroundPointDist;
    float servo1_GroundPointDist;
    void CalculateIK(IKAxis IKAxis)
    {
        var servoGroup = IKAxis.servoGroup;

        if (IKAxis.fixedAngleServo)
        {

            var targetOffset = IKAxis.fixedAngleServo.servoBase.InverseTransformPoint(IKAxis.fixedAngleServo.servoBase.transform.position + new Vector3(0, -100, 0));
            var angle = Math.Atan2(targetOffset.z, targetOffset.y);
            angle *= (180 / Math.PI);
            IKAxis.fixedAngleServo.SetServo((float)angle);
            //IKAxis.fixedAngleServo.LookDown();
        }

        if (IKAxis.servo0)
        {
            if (IKAxis.fixedAngleServo)
            {
                //   Debug.Log("run servo 0");
                var yDif = IKAxis.servo0.groundPoint.position.y - gait.position.y;
                Debug.Log(yDif);
                var limbDist = Vector3.Distance(IKAxis.fixedAngleServo.transform.position, IKAxis.fixedAngleServo.groundPoint.position);
                var targetPos = gait.position;// + new Vector3(0, limbDist, 0);
                if (testTarget)
                    testTarget.position = targetPos;

                var targetOffset = IKAxis.servo0.servoBase.InverseTransformPoint(targetPos);
                var angle = Math.Atan2(targetOffset.z, targetOffset.y);
                angle *= (180 / Math.PI);

                // angle += IKAxis.fixedAngleServo.setAngle;
                // angle = -IKAxis.fixedAngleServo.setAngle;

                IKAxis.servo0.SetServo((float)angle);// + IKAxis.fixedAngleServo.setAngle);
            }
            else
            {
                //   Debug.Log("run servo 0");
                var targetOffset = IKAxis.servo0.servoBase.InverseTransformPoint(gait.position);
                var angle = Math.Atan2(targetOffset.z, targetOffset.y);
                angle *= (180 / Math.PI);

                IKAxis.servo0.SetServo((float)angle);
            }


        }

        if (IKAxis.servo1)
        {
            // Debug.Log("run servo 2");
            var targetOffset = IKAxis.servo1.servoBase.InverseTransformPoint(gait.position);
            var angle0 = Math.Atan2(targetOffset.z, targetOffset.y);
            angle0 *= (180 / Math.PI);
            // float angle1 = LawOfCosines(IKAxis.servo1.limbLength, IKAxis.servo0.limbLength, Vector3.Distance(IKAxis.servo1.transform.position, gait.position));

            if (limbDistCheckCount == 0)
            {
                servo1_0Dist = Vector3.Distance(IKAxis.servo1.transform.position, IKAxis.servo0.transform.position);
                servo0_GroundPointDist = Vector3.Distance(IKAxis.servo0.transform.position, IKAxis.servo0.groundPoint.position);
                servo1_GroundPointDist = Vector3.Distance(IKAxis.servo1.transform.position, gait.position);
                limbDistCheckCount = 30;
            }
            limbDistCheckCount--;
            float angle1 = LawOfCosines(servo1_0Dist, servo0_GroundPointDist, servo1_GroundPointDist);
            //float angle1 = LawOfCosines(IKAxis.servo1.limbLength, Vector3.Distance(IKAxis.servo0.transform.position,gait.position), Vector3.Distance(IKAxis.servo1.transform.position, gait.position));

            if (IKAxis.servo1.invert)
            {
                angle1 -= (float)angle0;
            }
            else
            {
                angle1 += (float)angle0;
            }

            if (!float.IsNaN((float)angle1))
            {
                IKAxis.servo1.SetServo((float)angle1);
            }
        }





        // }

        // // for (int i = 0; i < servoGroup.Count; i++)
        ////  {
        //      var servo = servoGroup[i];
        //      if (i == 0)
        //      {
        //          if (servo.targetAngle != Vector3.down)
        //          {
        //              var targetOffset = servoGroup[0].servoBase.InverseTransformPoint(gait.position);
        //              var angle = Math.Atan2(targetOffset.z, targetOffset.y);
        //              angle *= (180 / Math.PI);
        //              servoGroup[0].SetServo((float)angle);
        //          }
        //          else
        //          {
        //              var targetOffset = servo.servoBase.InverseTransformPoint(Vector3.down);
        //              var angle = Math.Atan2(targetOffset.z, targetOffset.y);
        //              angle *= (180 / Math.PI);
        //              servoGroup[0].SetServo(-(float)angle);
        //          }
        //      }

        //      if (i == 1 || i == 2)
        //      {
        //          Debug.Log("run servo 2");
        //          var targetOffset = servoGroup[i].servoBase.InverseTransformPoint(gait.position);
        //          var angle0 = Math.Atan2(targetOffset.z, targetOffset.y);
        //          angle0 *= (180 / Math.PI);
        //          float angle1 = LawOfCosines(servoGroup[i].limbLength, servoGroup[i - 1].limbLength, Vector3.Distance(servoGroup[i].transform.position, gait.position));
        //          //float angle1 = LawOfCosines(Vector3.Distance(servoGroup[i].transform.position, servoGroup[0].transform.position), servoGroup[0].limbLength, Vector3.Distance(servoGroup[i].transform.position, gait.position));
        //          if (servo.invert)
        //          {
        //              angle1 -= (float)angle0;
        //          }
        //          else
        //          {
        //              angle1 += (float)angle0;
        //          }



        //          if (!float.IsNaN((float)angle1))
        //          {
        //              servoGroup[i].SetServo((float)angle1);
        //          }
        //          else
        //          {
        //              //  servoGroup[i].SetServo((float)angle0);
        //          }
        //      }
        // // }
    }
    public float LawOfCosines(float a, float b, float c)
    {
        var topEqu = (Math.Pow(c, 2) + Math.Pow(a, 2) - Math.Pow(b, 2));
        var bottomEqu = 2 * a * c;
        var angle = topEqu / bottomEqu;
        angle = (float)Math.Acos(angle);
        angle = (float)(angle * 180 / Math.PI);
        return (float)angle;
    }
    public void SetServos()
    {
        foreach (var servo in servosIK)
        {
            servo.SetServo();
        }
    }


    public void ActivateIK()
    {
        var newObject = Instantiate(Resources.Load("Gait", typeof(GameObject))) as GameObject;
        gait = newObject.transform;
        gait.position = limbController.limbMirror.limbEnd.position;
        gait.SetParent(transform);
        IKactive = true;
    }

    public void StoreGroupedServos()
    {
        Debug.Log("store grouped servos");
        IKAxisX = new IKAxis();
        IKAxisX.servoGroup = new List<RoboticServoIK>();
        IKAxisY = new IKAxis();
        IKAxisY.servoGroup = new List<RoboticServoIK>();
        IKAxisZ = new IKAxis();
        IKAxisZ.servoGroup = new List<RoboticServoIK>();
        foreach (var servo in servosIK)
        {
            if (!servo.disabled)
            {
                switch (servo.limbAxis)
                {
                    case LimbController.LimbAxis.X:
                        IKAxisX.servoGroup.Add(servo);
                        break;
                    case LimbController.LimbAxis.Y:
                        IKAxisY.servoGroup.Add(servo);
                        break;
                    case LimbController.LimbAxis.Z:
                        IKAxisZ.servoGroup.Add(servo);
                        break;
                }
            }
            else
            {
                Debug.Log("Did not add to servo list " + servo.name);
            }
        }

        IKAxisX.servoGroup.Reverse();
        IKAxisY.servoGroup.Reverse();
        IKAxisZ.servoGroup.Reverse();

        CalculateServoGroupOffset(ref IKAxisX);
        CalculateServoGroupOffset(ref IKAxisY);
        CalculateServoGroupOffset(ref IKAxisZ);

        foreach (var item in IKAxisX.servoGroup)
        {
            Debug.Log(item.name + "dddddddd");
        }
    }

    void CalculateServoGroupOffset(ref IKAxis IKAxis)
    {
        Debug.Log("servo axis count " + IKAxis.servoGroup.Count + " ");
        var servoGroup = IKAxis.servoGroup;
        for (int i = 0; i < servoGroup.Count; i++)
        {
            if (i == 0)
            {
                servoGroup[i].limbLength = Vector3.Distance(servoGroup[i].transform.position, servoGroup[i].groundPoint.position);
            }
            else
            {
                servoGroup[i].limbLength = Vector3.Distance(servoGroup[i].transform.position, servoGroup[i - 1].transform.position);
                var limbOffset = servoGroup[i].servoBase.InverseTransformPoint(servoGroup[i - 1].transform.position);
                var tempAngle = (float)(Math.Atan2(limbOffset.z, limbOffset.y));
                tempAngle = (float)(tempAngle * 180 / Math.PI);
                servoGroup[i].targetOffset = tempAngle;
            }
        }

        if (servoGroup.Count > 0)
        {
            if (servoGroup[0].targetAngle != Vector3.zero)
            {
                Debug.Log("0 joint has set angle");
                IKAxis.fixedAngleServo = servoGroup[0];
                if (servoGroup.Count > 1)
                {
                    IKAxis.servo0 = servoGroup[1];
                }
                if (servoGroup.Count > 2)
                {
                    IKAxis.servo1 = servoGroup[2];
                }
            }
            else
            {
                if (servoGroup.Count > 0)
                {
                    IKAxis.servo0 = servoGroup[0];
                    Debug.Log("set servo0");
                }
                if (servoGroup.Count > 1)
                {
                    IKAxis.servo1 = servoGroup[1];
                }
            }
        }

    }
    public void ConvertToIKLimb(RoboticLimbMirror mirrorLimb)
    {
        Debug.Log("create ik leg");
        var servosMirror = servos;
        servosIK = new List<RoboticServoIK>();
        var newServos = new List<RoboticServo>();
        //Convert the mirror servos to Ik servos
        foreach (var servo in servosMirror)
        {
            var newServoIK = servo.gameObject.AddComponent(typeof(RoboticServoIK)) as RoboticServoIK;
            servo.Clone(newServoIK);
            servosIK.Add(newServoIK);
            newServos.Add(newServoIK);
        }
        for (int i = 0; i < servosMirror.Length; i++)
        {
            if (servosMirror[i].servoParent)
            {
                servosIK[i].servoParent = servosMirror[i].servoParent.gameObject.GetComponent<RoboticServoIK>();
            }
            if (servosMirror[i].servoChild)
            {
                servosIK[i].servoChild = servosMirror[i].servoChild.gameObject.GetComponent<RoboticServoIK>();
            }
        }
        foreach (var servo in servosMirror)
        {
            Destroy(servo);
        }
        //Link the RoboticServoMirror and RoboticServoIk components
        foreach (var servo in servosIK)
        {
            var mirrorPart = mirrorLimb.FindServo(servo.hostPart.ID);
            servo.SetMirrorServo(mirrorPart.GetComponent<RoboticServoMirror>());
        }
        servosMirror = new List<RoboticServoMirror>().ToArray();
        servos = newServos.ToArray();
        DisableParts();
    }
}
