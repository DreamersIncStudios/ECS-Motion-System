using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LightProbeGroup))]
public class LightProbesTetrahedralGrid : MonoBehaviour
{
    // Common
    public float m_Side = 1.0f;
    public float m_Radius = 5.0f;
    public float m_InnerRadius = 0.1f;
    public float m_Height = 2.0f;
    public uint m_Levels = 3;
    const float kMinSide = 0.05f;
    const float kMinHeight = 0.05f;
    const float kMinInnerRadius = 0.1f;
    const uint kMinIterations = 4;
    public void OnValidate()
    {
        m_Side = Mathf.Max(kMinSide, m_Side);
        m_Height = Mathf.Max(kMinHeight, m_Height);
        if (m_InnerRadius < kMinInnerRadius)
        {
            TriangleProps props = new TriangleProps(m_Side);
            m_Radius = Mathf.Max(props.circumscribedCircleRadius + 0.01f, m_Radius);
        }
        else
        {
            m_Radius = Mathf.Max(0.1f, m_Radius);
            m_InnerRadius = Mathf.Min(m_Radius, m_InnerRadius);
        }
        Generate();
    }
    struct TriangleProps
    {
        public TriangleProps(float triangleSide)
        {
            side = triangleSide;
            halfSide = side / 2.0f;
            height = Mathf.Sqrt(3.0f) * side / 2.0f;
            inscribedCircleRadius = Mathf.Sqrt(3.0f) * side / 6.0f;
            circumscribedCircleRadius = 2.0f * height / 3.0f;
        }
        public float side;
        public float halfSide;
        public float height;
        public float inscribedCircleRadius;
        public float circumscribedCircleRadius;
    };

    private TriangleProps m_TriangleProps;
    public void Generate()
    {
        LightProbeGroup lightProbeGroup = this.GetComponent<LightProbeGroup>();
        List<Vector3> positions = new List<Vector3>();
        m_TriangleProps = new TriangleProps(m_Side);
        if (m_InnerRadius < kMinInnerRadius)
            GenerateCylinder(m_TriangleProps, m_Radius, m_Height, m_Levels, positions);
        else
            GenerateRing(m_TriangleProps, m_Radius, m_InnerRadius, m_Height, m_Levels, positions);
        lightProbeGroup.probePositions = positions.ToArray();
    }
    static void AttemptAdding(Vector3 position, Vector3 center, float distanceCutoffSquared, List<Vector3> outPositions)
    {
        if ((position - center).sqrMagnitude < distanceCutoffSquared)
            outPositions.Add(position);
    }
    uint CalculateCylinderIterations(TriangleProps props, float radius)
    {
        int iterations = Mathf.CeilToInt((radius + props.height - props.inscribedCircleRadius) / props.height);
        if (iterations > 0)
            return (uint)iterations;
        return 0;
    }
    void GenerateCylinder(TriangleProps props, float radius, float height, uint levels, List<Vector3> outPositions)
    {
        uint iterations = CalculateCylinderIterations(props, radius);
        float distanceCutoff = radius;
        float distanceCutoffSquared = distanceCutoff * distanceCutoff;
        Vector3 up = new Vector3(props.circumscribedCircleRadius, 0.0f, 0.0f);
        Vector3 leftDown = new Vector3(-props.inscribedCircleRadius, 0.0f, -props.halfSide);
        Vector3 rightDown = new Vector3(-props.inscribedCircleRadius, 0.0f, props.halfSide);
        for (uint l = 0; l < levels; l++)
        {
            float tLevel = levels == 1 ? 0 : (float)l / (float)(levels - 1);
            Vector3 center = new Vector3(0.0f, tLevel * height, 0.0f);
            if (l % 2 == 0)
            {
                for (uint i = 0; i < iterations; i++)
                {
                    Vector3 upCorner = center + up + (float)i * up * 2.0f * 3.0f / 2.0f;
                    Vector3 leftDownCorner = center + leftDown + (float)i * leftDown * 2.0f * 3.0f / 2.0f;
                    Vector3 rightDownCorner = center + rightDown + (float)i * rightDown * 2.0f * 3.0f / 2.0f;
                    AttemptAdding(upCorner, center, distanceCutoffSquared, outPositions);
                    AttemptAdding(leftDownCorner, center, distanceCutoffSquared, outPositions);
                    AttemptAdding(rightDownCorner, center, distanceCutoffSquared, outPositions);
                    Vector3 leftDownUp = upCorner - leftDownCorner;
                    Vector3 upRightDown = rightDownCorner - upCorner;
                    Vector3 rightDownLeftDown = leftDownCorner - rightDownCorner;
                    uint subdiv = 3 * i + 1;
                    for (uint s = 1; s < subdiv; s++)
                    {
                        Vector3 leftDownUpSubdiv = leftDownCorner + leftDownUp * (float)s / (float)subdiv;
                        AttemptAdding(leftDownUpSubdiv, center, distanceCutoffSquared, outPositions);
                        Vector3 upRightDownSubdiv = upCorner + upRightDown * (float)s / (float)subdiv;
                        AttemptAdding(upRightDownSubdiv, center, distanceCutoffSquared, outPositions);
                        Vector3 rightDownLeftDownSubdiv = rightDownCorner + rightDownLeftDown * (float)s / (float)subdiv;
                        AttemptAdding(rightDownLeftDownSubdiv, center, distanceCutoffSquared, outPositions);
                    }
                }
            }
            else
            {
                for (uint i = 0; i < iterations; i++)
                {
                    Vector3 upCorner = center + (float)i * (2.0f * up * 3.0f / 2.0f);
                    Vector3 leftDownCorner = center + (float)i * (2.0f * leftDown * 3.0f / 2.0f);
                    Vector3 rightDownCorner = center + (float)i * (2.0f * rightDown * 3.0f / 2.0f);
                    AttemptAdding(upCorner, center, distanceCutoffSquared, outPositions);
                    AttemptAdding(leftDownCorner, center, distanceCutoffSquared, outPositions);
                    AttemptAdding(rightDownCorner, center, distanceCutoffSquared, outPositions);
                    Vector3 leftDownUp = upCorner - leftDownCorner;
                    Vector3 upRightDown = rightDownCorner - upCorner;
                    Vector3 rightDownLeftDown = leftDownCorner - rightDownCorner;
                    uint subdiv = 3 * i;
                    for (uint s = 1; s < subdiv; s++)
                    {
                        Vector3 leftDownUpSubdiv = leftDownCorner + leftDownUp * (float)s / (float)subdiv;
                        AttemptAdding(leftDownUpSubdiv, center, distanceCutoffSquared, outPositions);
                        Vector3 upRightDownSubdiv = upCorner + upRightDown * (float)s / (float)subdiv;
                        AttemptAdding(upRightDownSubdiv, center, distanceCutoffSquared, outPositions);
                        Vector3 rightDownLeftDownSubdiv = rightDownCorner + rightDownLeftDown * (float)s / (float)subdiv;
                        AttemptAdding(rightDownLeftDownSubdiv, center, distanceCutoffSquared, outPositions);
                    }
                }
            }
        }
    }
    void GenerateRing(TriangleProps props, float radius, float innerRadius, float height, uint levels, List<Vector3> outPositions)
    {
        float chordLength = props.side;
        float angle = Mathf.Clamp(2.0f * Mathf.Asin(chordLength / (2.0f * radius)), 0.01f, 2.0f * Mathf.PI);
        uint slicesAtRadius = (uint)Mathf.FloorToInt(2.0f * Mathf.PI / angle);
        uint layers = (uint)Mathf.Max(Mathf.Ceil((radius - innerRadius) / props.height), 0.0f);
        for (uint level = 0; level < levels; level++)
        {
            float tLevel = levels == 1 ? 0 : (float)level / (float)(levels - 1);
            float y = height * tLevel;
            float iterationOffset0 = level % 2 == 0 ? 0.0f : 0.5f;
            for (uint layer = 0; layer < layers; layer++)
            {
                float tLayer = layers == 1 ? 1.0f : (float)layer / (float)(layers - 1);
                float tIterations = (tLayer * (radius - innerRadius) + innerRadius - kMinInnerRadius) / (radius - kMinInnerRadius);
                uint slices = (uint)Mathf.CeilToInt(Mathf.Lerp(kMinIterations, slicesAtRadius, tIterations));
                float x = innerRadius + (radius - innerRadius) * tLayer;
                Vector3 position = new Vector3(x, y, 0.0f);
                float layerSliceOffset = layer % 2 == 0 ? 0.0f : 0.5f;
                for (uint slice = 0; slice < slices; slice++)
                {
                    Quaternion rotation = Quaternion.Euler(0.0f, (slice + iterationOffset0 + layerSliceOffset) * 360.0f / (float)slices, 0.0f);
                    outPositions.Add(rotation * position);
                }
            }
        }
    }
}