﻿using System;
using System.Collections.Generic;
using GameControllers;
using OknaaEXTENSIONS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PuzzleSystem {
    public class TileGroup : MonoBehaviour {
        public List<Tile> tiles = new List<Tile>();
        [SerializeField] private EventTrigger eventTrigger;

        private Puzzle _puzzle;
        private Vector3 _originalPosition;
        private Transform _originalParent;
        private int _startIndex;
        private TileMovement _movementScript;

        private bool IsSnapped => _movementScript.IsSnapped;

        private bool IsDragged {
            set => _movementScript.IsDragged = value;
        }
        
        
        private void Awake() {
            _movementScript = GetComponent<TileMovement>();
        }

        private void Start() {
            SetEventTrigger(EventTriggerType.PointerDown, OnPointerDown);
            SetEventTrigger(EventTriggerType.Drag, OnDrag);
            SetEventTrigger(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void OnPointerDown(BaseEventData data) {
            var tileTransform = transform;
            _originalPosition = tileTransform.position;
            _originalParent = tileTransform.parent;
            _startIndex = transform.GetSiblingIndex();
            transform.SetParent(_puzzle.tileShadowsContainer);
        }

        private void OnDrag(BaseEventData data) {
            transform.position = ((PointerEventData)data).position;
            IsDragged = true;
        }

        private void OnPointerUp(BaseEventData data) {
            IsDragged = false;
            if (IsSnapped) return;
            
            transform.SetParent(_originalParent);
            transform.SetPositionAndRotation(_originalPosition, Quaternion.identity);
            transform.SetSiblingIndex(_startIndex);
            
            RefreshScrollListUI();
        }
        
        private void RefreshScrollListUI() {
            var tileContainer = _puzzle.tilesContainer;
            tileContainer.GetChild(1).transform.SetAsFirstSibling();
        }

        public void Init(List<Tile> groupedTiles, Puzzle puzzle, Tile originTile) {
            if (_movementScript == null) _movementScript = GetComponent<TileMovement>();
            _movementScript.OriginTile = originTile;
            tiles = groupedTiles;
            _puzzle = puzzle;
            name = "TileGroup " + (tiles.Count);

            ResizeTiles();
        }

        private void ResizeTiles() {
            foreach (var tile in tiles) {
                tile.ResizeToOriginalSize();
            }
        }

        private void SetEventTrigger(EventTriggerType type, UnityAction<BaseEventData> action) {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(action);
            eventTrigger.triggers.Add(entry);
        }
    }
}