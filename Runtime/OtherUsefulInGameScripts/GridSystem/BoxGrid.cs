using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace TCUtils {

    public class BoxGridDesc {

        public float3 cellSize;

        public RigidTransform GridTransform {
            get => gridTransform;
            set {
                gridTransform = value;
                inverseGridTransform = math.inverse(gridTransform);
            }
        }

        private RigidTransform gridTransform;
        private RigidTransform inverseGridTransform;

        public BoxGridDesc() : this(new float3(1, 1, 1)) {}

        public BoxGridDesc(float3 cellSize) : this(cellSize, RigidTransform.identity) {}

        public BoxGridDesc(float3 cellSize, RigidTransform transform) {
            this.cellSize = cellSize;
            GridTransform = transform;
        }

        public int3 WorldPositionToGridCoordinate(float3 worldPosition) {
            var localPos = math.transform(inverseGridTransform, worldPosition);
            return LocalPositionToGridCoordinate(localPos);
        }

        public float3 GridCoordinateToWorldPosition(int3 gridCoordinate) {
            return math.transform(gridTransform, GridIndexToLocalPosition(gridCoordinate));
        }

        public int3 LocalPositionToGridCoordinate(float3 localPosition) {
            var result = math.floor(localPosition / cellSize);
            return (int3)result;
        }

        public float3 GridIndexToLocalPosition(int3 gridCoordinate) {
            return gridCoordinate * cellSize + cellSize / 2;
        }
    }

    public class BoxGrid<TCell> {

        private readonly TCell[] data;

        private BoxGridDesc GridDesc { get; set; }
        public int3 GridTotalSize { get; private set; }

        public BoxGrid(int3 gridTotalSize, TCell defaultCellValue) : this(gridTotalSize, new float3(1, 1, 1), defaultCellValue) {}

        public BoxGrid(int3 gridTotalSize, float3 cellSize, TCell defaultCellValue) {

            // init grid data
            Debug.Assert(gridTotalSize.x > 0 & gridTotalSize.y > 0 & gridTotalSize.z > 0, "GridSize need to be larger than 0!!");
            GridTotalSize = math.max(new int3(1, 1, 1), gridTotalSize);
            data = new TCell[GridTotalSize.x * GridTotalSize.y * GridTotalSize.z];
            for (var i = 0; i < data.Length; ++i) {
                data[i] = defaultCellValue;
            }

            GridDesc = new BoxGridDesc(cellSize);
        }

        public TCell[] GetData() {
            return data;
        }

        public TCell GetDataAt(float3 worldPos) {
            var coordinate = GridDesc.WorldPositionToGridCoordinate(worldPos);
            CheckCoordinateInRange(coordinate);

            return data[GridCoordinateToDataIndex(coordinate)];
        }

        public void SetDataAt(float3 worldPos, TCell cellData) {
            var coordinate = GridDesc.WorldPositionToGridCoordinate(worldPos);
            CheckCoordinateInRange(coordinate);

            data[GridCoordinateToDataIndex(coordinate)] = cellData;
        }

        private bool CheckCoordinateInRange(int3 gridCoordinate, bool assert = true) {
            var isInRange = gridCoordinate.x >= 0 && gridCoordinate.y >= 0 && gridCoordinate.z >= 0;
            isInRange &= gridCoordinate.x < GridTotalSize.x && gridCoordinate.y < GridTotalSize.y && gridCoordinate.z < GridTotalSize.z;
            Debug.Assert(!assert || isInRange, $"GridCoordinate not in range: {gridCoordinate}");
            return isInRange;
        }

        private int GridCoordinateToDataIndex(int3 gridCoordinate) {
            var size = GridTotalSize;
            return gridCoordinate.x + gridCoordinate.y * size.x + gridCoordinate.z * size.x * size.y;
        }

        private int3 DataIndexToGridCoordinate(int dataIndex) {
            var size = GridTotalSize;
            var s1 = size.x * size.y;
            int3 result;
            result.z = dataIndex / s1;
            dataIndex -= s1 * result.z;

            result.y = dataIndex / size.x;
            result.x = dataIndex - size.x * result.y;

            return result;
        }
    }
}