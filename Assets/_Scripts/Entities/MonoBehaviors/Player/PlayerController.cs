﻿using System;
using UnityEngine;
using Rewired;

namespace Entities.Player {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {

        [PlayerIdProperty(typeof(RewiredConsts.Player))]
        public int player;

        public int   numberOfLives         = 1;
        public float walkingSpeed          = 5f;
        public int   movementRotationSpeed = 100;
        public int   shootingRotationSpeed = 200;

        private Rigidbody      _rb;
        private Animator       _animator;
        private Core.GamePhase _phase;

        public Rewired.Player PlayerInputs { get; protected set; }

        public bool HasLives {
            get { return numberOfLives > 0; }
        }

        // ========================================================
        // ========================================================


        protected void Awake() {
            PlayerInputs = ReInput.players.GetPlayer(player); // Get the MainPlayer's inputs
            _rb          = GetComponent<Rigidbody>();
        }


        private void Update() {
            MoveForward();
            CheckForRotation();
        }


        // ========================================================
        // ========================================================


        /// <summary>
        /// Move the gameobject forward in local space
        /// </summary>
        private void MoveForward() {
            Vector3 locVel = transform.InverseTransformDirection(_rb.velocity);
            if ( _phase == Core.GamePhase.Moving )
                locVel = new Vector3(0, 0, walkingSpeed); // Move forward in local space
            else
                locVel = new Vector3(0, 0, 0);

            _rb.velocity = transform.TransformDirection(locVel);

            // Reset Angular Velocity (Manual movement only)
            _rb.angularVelocity = Vector3.zero;
        }


        /// <summary>
        /// Spawn Player at given position
        /// </summary>
        /// <param name="pos"></param>
        public void SpawnAt(Vector3 pos) {
            transform.position = pos;
            enabled = true;
        }


        /// <summary>
        /// Kill Event
        /// External call
        /// </summary>
        public void Kill() {
            numberOfLives--;
            // Spawn particles ???
            enabled = false;
        }


        /// <summary>
        /// Receive message with the current gamePhase
        /// </summary>
        /// <param name="phase"></param>
        public void ChangeGamePhase(Core.GamePhase phase) { _phase = phase; }


        /// <summary>
        /// Check inputs for rotating the player
        /// </summary>
        private void CheckForRotation() {
            float horizontal = PlayerInputs.GetAxisRaw(RewiredConsts.Action.Horizontal);
            float vertical   = PlayerInputs.GetAxisRaw(RewiredConsts.Action.Vertical);

            // When moving (Type 1)
            if ( _phase == Core.GamePhase.Moving ) {
                if ( !Mathf.Approximately(horizontal, 0f) )
                    transform.Rotate(Vector3.up * movementRotationSpeed * Mathf.Sign(horizontal) * Time.deltaTime);
            } else if ( _phase == Core.GamePhase.Shooting ) {
                if ( !Mathf.Approximately(horizontal, 0f) )
                    transform.Rotate(Vector3.up * shootingRotationSpeed * Mathf.Sign(horizontal) * Time.deltaTime);
            }
        }

    }
}