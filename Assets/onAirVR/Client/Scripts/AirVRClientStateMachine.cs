/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AirVRClientStateMachine {
    protected abstract class State {
        public State(AirVRClientStateMachine owner) {
            this.owner = owner;
        }

        protected AirVRClientStateMachine owner { get; private set; }

        public virtual void Update(Context context, float deltaTime) {}

        public virtual void Connected(Context context, bool appFocused)     { Debug.Assert(false); }
        public virtual void PlayRequested(Context context) {}
        public virtual void StopRequested(Context context) {}
        public virtual void AppFocused(Context context) {}
        public virtual void AppUnfocused(Context context) {}
        public virtual void AppPaused(Context context) {}
        public virtual void AppResumed(Context context) {}

        public virtual void Disconnected(Context context) {
            owner.transitTo(owner.stateDisconnected);
        }
    }

    protected class StateDisconnected : State {
        public StateDisconnected(AirVRClientStateMachine owner) : base(owner) {}
        
        public override void Connected(Context context, bool appFocused) {
            if (appFocused) {
                owner.transitTo(owner.stateReady);
            }
            else {
                owner.transitTo(owner.stateUnfocused);
            }
        }

        public override void Disconnected(Context context) {}
    }

    protected class StateReady : State {
        public StateReady(AirVRClientStateMachine owner) : base(owner) {}
        
        public override void PlayRequested(Context context) {
            owner.transitTo(owner.statePlaying);
            context.RequestPlay();
        }

        public override void AppUnfocused(Context context) {
            owner.transitTo(owner.stateUnfocused);
        }
    }

    protected class StateUnfocused : State {
        public StateUnfocused(AirVRClientStateMachine owner) : base(owner) {}
        
        public override void AppFocused(Context context) {
            owner.transitTo(owner.stateReady);
        }

        public override void PlayRequested(Context context) {
            owner.transitTo(owner.stateInactive);
        }
    }

    protected class StatePlaying : State {
        public StatePlaying(AirVRClientStateMachine owner) : base(owner) {}

        public override void StopRequested(Context context) {
            owner.transitTo(owner.stateReady);
            context.RequestStop();
        }

        public override void AppUnfocused(Context context) {
            owner.transitTo(owner.stateInactive);
            context.RequestStop();
        }

        public override void AppPaused(Context context) {
            owner.transitTo(owner.statePaused);
            context.RequestStop();
        }
    }

    protected class StateInactive : State {
        public StateInactive(AirVRClientStateMachine owner) : base(owner) {}

        public override void StopRequested(Context context) {
            owner.transitTo(owner.stateUnfocused);
        }

        public override void AppFocused(Context context) {
            owner.transitTo(owner.statePlaying);
            context.RequestPlay();
        }

        public override void AppPaused(Context context) {
            owner.transitTo(owner.statePaused);
        }
    }

    protected class StatePaused : State {
        public StatePaused(AirVRClientStateMachine owner) : base(owner) {}

        public override void AppResumed(Context context) {
            owner.transitTo(owner.stateResuming);
        }
    }

    protected class StateResuming : State {
        public StateResuming(AirVRClientStateMachine owner, float delayToResume) : base(owner) {
            _delayToResume = delayToResume;
            _remainingToResume = delayToResume;
        }

        private readonly float _delayToResume;
        private float _remainingToResume;

        public override void Update(Context context, float deltaTime) {
            _remainingToResume -= deltaTime;
            if (_remainingToResume <= 0.0f) {
                _remainingToResume = _delayToResume;
                
                owner.transitTo(owner.statePlaying);
                context.RequestPlay();
            }
        }

        public override void StopRequested(Context context) {
            _remainingToResume = _delayToResume;
            
            owner.transitTo(owner.stateReady);
        }

        public override void AppUnfocused(Context context) {
            _remainingToResume = _delayToResume;
            
            owner.transitTo(owner.stateInactive);
        }

        public override void Disconnected(Context context) {
            _remainingToResume = _delayToResume;
            
            base.Disconnected(context);
        }
    }

    public interface Context {
        void RequestPlay();
        void RequestStop();
    }

    public AirVRClientStateMachine(Context context, float delayToResume) {
        _context = context;
        stateDisconnected = new StateDisconnected(this);
        stateReady = new StateReady(this);
        stateUnfocused = new StateUnfocused(this);
        statePlaying = new StatePlaying(this);
        stateInactive = new StateInactive(this);
        statePaused = new StatePaused(this);
        stateResuming = new StateResuming(this, delayToResume);

        _state = stateDisconnected;
        _lastAppFocused = true;
    }

    private Context _context;
    private State _state;
    private bool _lastAppFocused;

    protected StateDisconnected stateDisconnected    { get; private set; }
    protected StateReady stateReady                  { get; private set; }
    protected StateUnfocused stateUnfocused          { get; private set; }
    protected StatePlaying statePlaying              { get; private set; }
    protected StateInactive stateInactive            { get; private set; }
    protected StatePaused statePaused                { get; private set; }
    protected StateResuming stateResuming            { get; private set; }

    protected void transitTo(State state) {
        _state = state;
    }

    public void TriggerConnected() {
        _state.Connected(_context, _lastAppFocused);
    }

    public void TriggerPlayRequested() {
        _state.PlayRequested(_context);
    }

    public void TriggerStopRequested() {
        _state.StopRequested(_context);
    } 

    public void TriggerDisconnected() {
        _state.Disconnected(_context);
    }

    public void Update(bool isAppFocused, float deltaTime) {
        if (_lastAppFocused != isAppFocused) {
            if (isAppFocused) {
                _state.AppFocused(_context);
            }
            else {
                _state.AppUnfocused(_context);
            }
            _lastAppFocused = isAppFocused;
        }
        
        _state.Update(_context, deltaTime);
    }

    public void UpdatePauseStatus(bool pauseStatus) {
        if (pauseStatus) {
            _state.AppPaused(_context);
        }
        else {
            _state.AppResumed(_context);
        }
    }
}
