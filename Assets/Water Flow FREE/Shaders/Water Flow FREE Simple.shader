Shader "Water Flow FREE/Water Flow FREE Simple" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Diffuse", Color) = (1,1,1,0.75)
		_MoveSpeedU ("U Move Speed", Range(-6,6)) = 0.5
		_MoveSpeedV ("V Move Speed", Range(-6,6)) = 0.5
     		_CutOff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags {
				"IgnoreProjector"="True" 
				"Queue"="Transparent" 
				"RenderType"="Transparent"
		} 
		ZWrite Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Color;
		fixed _MoveSpeedU;
		fixed _MoveSpeedV;
		float _CutOff;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
		
			fixed2 MoveScrolledUV = IN.uv_MainTex;
			
			fixed MoveU = _MoveSpeedU * _Time;
			fixed MoveV = _MoveSpeedV * _Time;
			
			MoveScrolledUV += fixed2(MoveU, MoveV);
		
			half4 c = tex2D (_MainTex, MoveScrolledUV);
			o.Albedo = c.rgb * _Color;
			if ((c.r + c.g + c.b) / 3.0 < _CutOff) {
				o.Alpha = 0.0;
			} else {
				o.Alpha = 1.0;
			}
		}
		ENDCG
	} 
	Fallback "Transparent/VertexLit"
}
