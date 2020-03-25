Shader "LidarSensor/Depth"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag

			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 position_in_world_space : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				output.pos = UnityObjectToClipPos(input.vertex);
				output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}

			float4 frag(vertexOutput input) : SV_Target
			{
				float4 cameraPosition = float4(_WorldSpaceCameraPos, 1.0);
				float dist = distance(input.position_in_world_space , cameraPosition);
				return float4(dist, dist, dist, dist);
			}

			ENDCG
		}
	}
}
