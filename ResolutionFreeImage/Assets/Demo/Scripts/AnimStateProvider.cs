using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResFreeImage.Demo {

    public class AnimStateProvider : StateMachineBehaviour
    {
        public class StateInfo {
            public Animator animator;
            public AnimatorStateInfo stateInfo;
            public int layerIndex;
        }

        public string userTag;

        [System.Serializable]
        public class AnimStateEvent : UnityEvent<string, StateInfo> {}
        
        public AnimStateEvent _onStateEnterEvent = new AnimStateEvent();
        public AnimStateEvent _onStateExitEvent = new AnimStateEvent();

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log(userTag + " OnStateEnter");
            var info = new StateInfo();
            info.animator = animator;
            info.stateInfo = stateInfo;
            info.layerIndex = layerIndex;
            _onStateEnterEvent.Invoke(userTag, info);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log(userTag + " OnStateExit");
            var info = new StateInfo();
            info.animator = animator;
            info.stateInfo = stateInfo;
            info.layerIndex = layerIndex;
            _onStateExitEvent.Invoke(userTag, info);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}