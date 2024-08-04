using System;
using UnityEditor;
using UnityEngine;

namespace ActiveRagdoll {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class AnimationModule : Module {
        [Header("--- BODY ---")]
        /// <summary> Required to set the target rotations of the joints </summary>
        private Quaternion[] _initialJointsRotation;
        private ConfigurableJoint[] _joints;
        private Transform[] _animatedBones;
        private AnimatorHelper _animatorHelper;
        public Animator Animator { get; private set; }

        [Header("--- INVERSE KINEMATICS ---")]
        public bool _enableIK = true;


        [Space(10)]
        [Tooltip("The limits of the arms direction. How far down/up should they be able to point?")]
        public float minArmsAngle = -70;
        public float maxArmsAngle = 100;
        [Tooltip("The limits of the look direction. How far down/up should the character be able to look?")]
        public float minLookAngle = -50, maxLookAngle = 60;

        [Space(10)]
        [Tooltip("The vertical offset of the look direction in reference to the target direction.")]
        public float lookAngleOffset;
        [Tooltip("The vertical offset of the arms direction in reference to the target direction.")]
        public float armsAngleOffset;
        [Tooltip("Defines the orientation of the hands")]
        public float handsRotationOffset = 0;

        [Space(10)]
        [Tooltip("How far apart to place the arms")]
        public float armsHorizontalSeparation = 0.75f;

        [Tooltip("The distance from the body to the hands in relation to how high/low they are. " +
                 "Allows to create more realistic movement patterns.")]
        public AnimationCurve armsDistance;

        private Vector3 _armsDir;
        private Transform _animTorso, _chest;
        private float _targetDirVerticalPercent;



        private void Start() {
            _joints = _activeRagdoll.Joints;
            _animatedBones = _activeRagdoll.AnimatedBones;
            _animatorHelper = _activeRagdoll.AnimatorHelper;
            Animator = _activeRagdoll.AnimatedAnimator;

            _initialJointsRotation = new Quaternion[_joints.Length];
            for (int i = 0; i < _joints.Length; i++) {
                _initialJointsRotation[i] = _joints[i].transform.localRotation;
            }
        }

        void FixedUpdate() {
            UpdateJointTargets();
            UpdateIK();
        }

        /// <summary> Makes the physical bones match the rotation of the animated ones </summary>
        private void UpdateJointTargets() {
            for (int i = 0; i < _joints.Length; i++) {
                ConfigurableJointExtensions.SetTargetRotationLocal(_joints[i], _animatedBones[i + 1].localRotation, _initialJointsRotation[i]);
            }
        }

        private void UpdateIK() {
            if (!_enableIK) {
                _animatorHelper.LeftArmIKWeight = 0;
                _animatorHelper.RightArmIKWeight = 0;
                _animatorHelper.LookIKWeight = 0;
                return;
            }
            _animatorHelper.LookIKWeight = 1;

            _animTorso = _activeRagdoll.AnimatedTorso;
            _chest = _activeRagdoll.GetAnimatedBone(HumanBodyBones.Spine);

            UpdateLookIK();
            UpdateArmsIK();
        }

        private void UpdateLookIK() {
            _animatorHelper.LookAtPoint(transform.forward);
        }

        private void UpdateArmsIK() {
            float armsVerticalAngle = _targetDirVerticalPercent * Mathf.Abs(maxArmsAngle - minArmsAngle) + minArmsAngle;
            armsVerticalAngle += armsAngleOffset;
            _armsDir = Quaternion.AngleAxis(-armsVerticalAngle, _animTorso.right).eulerAngles;

            float currentArmsDistance = armsDistance.Evaluate(_targetDirVerticalPercent);

            Vector3 armsMiddleTarget = _chest.position + _armsDir * currentArmsDistance;
            Vector3 upRef = Vector3.Cross(_armsDir, _animTorso.right).normalized;
            Vector3 armsHorizontalVec = Vector3.Cross(_armsDir, upRef).normalized;
            Quaternion handsRot = _armsDir != Vector3.zero? Quaternion.LookRotation(_armsDir, upRef) : Quaternion.identity;

            _animatorHelper.LeftHandTarget.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation / 2;
            _animatorHelper.RightHandTarget.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation / 2;
            _animatorHelper.LeftHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, 90 - handsRotationOffset);
            _animatorHelper.RightHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, -90 + handsRotationOffset);

            var armsUpVec = Vector3.Cross(_armsDir, _animTorso.right).normalized;
            _animatorHelper.LeftHandHint.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
            _animatorHelper.RightHandHint.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
        }

        /// <summary> Plays an animation using the animator. The speed doesn't change the actual
        /// speed of the animator, but a parameter of the same name that can be used to multiply
        /// the speed of certain animations. </summary>
        /// <param name="animation">The name of the animation state to be played</param>
        /// <param name="speed">The speed to be set</param>
        public void PlayAnimation(string animation, float speed = 1) {
            Animator.Play(animation);
            Animator.SetFloat("speed", speed);
        }
        
        public void UseLeftArm(float weight) {
            if (!_enableIK)
                return;

            _animatorHelper.LeftArmIKWeight = weight;
        }

        public void UseRightArm(float weight) {
            if (!_enableIK)
                return;

            _animatorHelper.RightArmIKWeight = weight;
        }
    }
} // namespace ActiveRagdoll