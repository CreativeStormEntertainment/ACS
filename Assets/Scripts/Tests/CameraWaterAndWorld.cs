using System.Collections.Generic;
using UnityEngine;

public class CameraWaterAndWorld : MonoBehaviour
{
    public Camera UICamera;
    public Transform CarrierFoamAndOffsetedEffects;
    public Camera SecondaryCamera;
    public float Offset;
    public float Speed; // 0-1
    public float MaxSpeed;

    //public List<Transform> EscortFoams => escortFoams;

    private Camera cam;
    private Camera camMain;
    private Transform camMainTrans;
    private Transform camTrans;
    //private List<Transform> escortFoams = new List<Transform>();

    private void Start()
    {
        cam = GetComponent<Camera>();
        camTrans = transform;

        camMain = Camera.main;
        camMainTrans = camMain.transform;
    }

    private void OnPreCull()
    {
        CarrierFoamAndOffsetedEffects.gameObject.SetActive(CameraManager.Instance.CurrentCameraView != ECameraView.PreviewCamera);
        if (CameraPositionFixer.Position.HasValue)
        {
            camMainTrans.position = CameraPositionFixer.Position.Value;
            CameraPositionFixer.Position = null;
        }
        if (CameraPositionFixer.Rotation.HasValue)
        {
            camMainTrans.rotation = CameraPositionFixer.Rotation.Value;
            CameraPositionFixer.Rotation = null;
        }
        if (cam)
        {
            if (TacticManager.Instance.Carrier.HasWaypoint)
            {
                Speed = Mathf.Clamp(HudManager.Instance.ShipSpeedup, 0f, 1f);
            }
            else
            {
                Speed = 0f;
            }
            float tempOffset = Speed * MaxSpeed * Time.deltaTime;
            Offset -= tempOffset;

            //temporary water shader should loop after demo at this point
            if (Offset < -15000f)
            {
                Offset = 0;
                CarrierFoamAndOffsetedEffects.position = new Vector3(CarrierFoamAndOffsetedEffects.position.x, CarrierFoamAndOffsetedEffects.position.y, 0f);
                //foreach (var foam in escortFoams)
                //{
                //    foam.position = new Vector3(foam.position.x, foam.position.y, 0f);
                //}
            }
            cam.fieldOfView = camMain.fieldOfView;
            UICamera.fieldOfView = cam.fieldOfView;
            SecondaryCamera.fieldOfView = UICamera.fieldOfView;

            var trans = CameraManager.Instance.GetCurrentCamera();
            Vector3 posTemp = trans.position;
            if (trans != camMainTrans && CameraManager.Instance.FixAttackCamera)
            {
                posTemp += camMainTrans.position;
                posTemp = new Vector3(posTemp.x / 2f, posTemp.y, posTemp.z / 2f);
            }

            posTemp.z += Offset;
            camTrans.position = posTemp;
            camTrans.rotation = trans.rotation;

            posTemp = CarrierFoamAndOffsetedEffects.position;
            posTemp.z -= tempOffset;
            CarrierFoamAndOffsetedEffects.position = posTemp;

            //foreach (var foam in escortFoams)
            //{
            //    posTemp = foam.position;
            //    posTemp.z -= tempOffset;
            //    foam.position = posTemp;
            //}
        }
    }
}
