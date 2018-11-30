using UnityEngine;

namespace CameraMessage
{
    public class CameraPositionAndMessage
    {
        public CameraPositionAndMessage(string text, Vector3 targetPos, Vector3 cameraDistance, Vector3 cameraAngle, float cameraFov)
        {
            Text = text;
            TargetPos = targetPos;
            CameraDistance = cameraDistance;
            CameraAngle = cameraAngle;
            CameraFov = cameraFov;
        }

        public Vector3 TargetPos { get; set; }
        public Vector3 CameraDistance { get; set; }
        public Vector3 CameraAngle { get; set; }
        public float CameraFov { get; set; }
        public string Text { get; set; }
        
    }
}
