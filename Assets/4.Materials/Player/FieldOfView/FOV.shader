Shader "Unlit/FOV"
{
    //각 속성 값들을 정의
    Properties
    {
        _Color ("Mask Color", Color) = (0,0,0,0.7)
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _PlayerForward ("Player Forward", Vector) = (0,0,1,0)
        _InnerRadius ("Inner Radius", Float) = 2.0
        _OuterRadius ("Outer Radius", Float) = 6.0
        _FOV ("Field of View (degrees)", Float) = 90.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _PlayerPos;
            float4 _PlayerForward;
            float _InnerRadius;
            float _OuterRadius;
            float _FOV;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // -1~1
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 기본 uv 좌표 (카메라 정면 기준)
                float2 uv = i.uv;

                // --- [1] Forward 방향으로 회전 보정 ---
                // _PlayerForward.xy를 기준으로 회전 각도 계산
                float2 forward = normalize(_PlayerForward.xz); // XZ 평면 사용
                float forwardAngle = atan2(forward.x, forward.y); // 라디안
                float cosA = cos(forwardAngle);
                float sinA = sin(forwardAngle);
                float2x2 rot = float2x2(cosA, -sinA, sinA, cosA);
                uv = mul(rot, uv); // 회전 적용

                // --- InnerRadius: 원형 영역 (UV 기준) ---
                bool inCircle = length(uv) * 32 < _InnerRadius;

                // --- OuterRadius + FOV: 부채꼴 영역 (UV 기준) ---
                float angle = degrees(atan2(uv.x, uv.y));
                float fovHalf = _FOV * 0.5;
                bool inFOV = length(uv) * 23 < _OuterRadius && abs(angle) < fovHalf;

                if (inCircle || inFOV)
                    return float4(0, 0, 0, 0);

                return _Color;
            }
            ENDCG
        }
    }
}