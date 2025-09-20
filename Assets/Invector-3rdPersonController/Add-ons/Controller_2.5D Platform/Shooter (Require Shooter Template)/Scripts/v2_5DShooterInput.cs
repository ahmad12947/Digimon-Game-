using UnityEngine;

namespace Invector.vCharacterController.v2_5D
{
    [vClassHeader("Shooter 2.5D Input")]
    public class v2_5DShooterInput : vShooterMeleeInput
    {
        [vEditorToolbar("2D Aiming")]
        public bool lookToCursorOnAiming = true;
        [vSeparator("AIM Z SETTINGS")]
        public bool aimZAxis = true;
        [vHideInInspector("aimZAxis")]
        public LayerMask aimZLayer = 1 << 0 | 1 << 9;
        [Tooltip("Leave empty to dont use tag filter")]
        public vTagMask aimZTags = new vTagMask("Enemy");
        [Tooltip("Max distance of the Aim Z Target (Ray cast distance). Zero(0) uses camera farClipPlane")]
        public float aimZRange = 10f;
        [Tooltip("This is a radius used to SphereCast. Zero(0) will use a simple raycast")]
        public float aimZRadius = 0;
        public bool debugAimRange;
        private v2_5DController _controller;
        Vector3 aimRefPoint;
        RaycastHit worldHit;

        public v2_5DController controller
        {
            get
            {
                if (cc && cc is v2_5DController && _controller == null)
                {
                    _controller = cc as v2_5DController;
                }

                return _controller;
            }
        }

        protected override void Start()
        {
            base.Start();
            vMousePositionHandler.Instance.SetMousePosition(cameraMain.WorldToScreenPoint(transform.position + Vector3.up + transform.forward * 100f));
            aimRefPoint = transform.InverseTransformPoint(aimAngleReference.transform.position);
        }

        protected override void UpdateAimPosition()
        {
            if (debugAimRange)
            {
                Debug.DrawLine(tpCamera.transform.position, tpCamera.transform.position + tpCamera.transform.forward * (AimZRange), Color.red, 0.1f);
            }
            if (!isAimingByInput || !controller)
            {
                return;
            }

            if (lookToCursorOnAiming)
            {
                UpdateAimPositionFromCursor();
            }
            else
            {
                UpdateAimPositionFromForward();
            }
        }

        protected virtual float AimZRange
        {
            get
            {
                float range = aimZRange;
                if (aimZRange > 0 && tpCamera)
                {

                    range = tpCamera.distance + aimZRange;
                }
                else if (tpCamera && tpCamera.targetCamera) range = tpCamera.targetCamera.farClipPlane;
                return range;
            }
        }

        protected virtual void UpdateAimPositionFromCursor()
        {
            Vector3 localPos = controller.localCursorPosition;
            localPos.x = 0;

            if (localPos.z < .1f)
            {
                localPos.z = .1f;
            }

            Vector3 wordPos = transform.TransformPoint(localPos);
            if (aimZAxis)
            {
                if (vMousePositionHandler.Instance.CastWorldMousePosition(aimZLayer, out worldHit, AimZRange, aimZRadius) && (aimZTags.Count == 0 || aimZTags.Contains(worldHit.collider.gameObject.tag)))
                {
                    wordPos = worldHit.point;
                }
            }

            Vector3 aimRef = transform.TransformPoint(aimRefPoint);
            Vector3 lookDirection = (wordPos - aimRef);
            lookDirection = lookDirection.normalized * (lookDirection.magnitude < shooterManager.minAimHitPointDistance ? shooterManager.minAimHitPointDistance : lookDirection.magnitude);

            wordPos = transform.TransformPoint(localPos);
            AimPosition = aimRef + lookDirection;
            headTrack.SetTemporaryLookPoint(wordPos, 0.1f);
        }

        protected override void UpdateHeadTrackLookPoint()
        {
            if (IsAiming && !isUsingScopeView)
            {
                headTrack.SetTemporaryLookPoint(AimPosition, 0.1f);
            }
        }

        protected virtual void UpdateAimPositionFromForward()
        {
            Vector3 wordPos = aimAngleReference.gameObject.transform.position + transform.forward * 100f;
            if (aimZAxis)
            {
                if (vMousePositionHandler.Instance.CastWorldMousePosition(aimZLayer, out worldHit, AimZRange, aimZRadius) && (aimZTags.Count == 0 || aimZTags.Contains(worldHit.collider.gameObject.tag)))
                {
                    wordPos = worldHit.point;
                }
            }
            AimPosition = wordPos;
        }

        //protected override void UpdateAimHud()
        //{
        //    //if (!shooterManager || !controlAimCanvas)
        //    //{
        //    //    return;
        //    //}

        //    //if (CurrentActiveWeapon == null)
        //    //{
        //    //    return;
        //    //}

        //    //controlAimCanvas.SetAimCanvasID(CurrentActiveWeapon.scopeID);
        //    //if (isAimingByInput)
        //    //{
        //    //    controlAimCanvas.SetWordPosition(AimPosition, aimConditions);
        //    //}
        //    //else
        //    //{
        //    //    controlAimCanvas.SetAimToCenter(true);
        //    //}
        //    //base.UpdateAimPosition
        //}

        protected override Vector3 targetArmAlignmentPosition => AimPosition;

        protected override Vector3 targetArmAlignmentDirection
        {
            get
            {
                return transform.forward;
            }
        }

        public override void ScopeViewInput()
        {
            ///Ignore ScopeView
        }
    }
}