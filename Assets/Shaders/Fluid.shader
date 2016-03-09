Shader "Custom/Fluid" 
{
	Properties 
	{
		_ID("ID", Int) = 0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", float) = 0.5
		_Speed("Speed", float) = 2.0
	}
	
	SubShader 
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient vertex:vert keepalpha exclude_path:deferred exclude_path:prepass noshadow nolightmap nodynlightmap nodirlightmap nometa noforwardadd
      	
      	struct Input 
      	{
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
      	
      	sampler2D _MainTex;
      	float _Alpha;
      	float _Speed;
      	
      	void vert(inout appdata_full v) 
      	{
        	v.texcoord.x += _Time.y * _Speed;
      	}
		
      	void surf(Input IN, inout SurfaceOutput o) 
      	{
      		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
      		o.Alpha = _Alpha;
      	
      		float3 light = IN.color.rgb;
      		float sun = IN.color.a;
      		float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2 * sun;
      		ambient = max(ambient, 0.0666);
      		ambient = max(ambient, light);
        	o.Emission = o.Albedo * ambient;
     	}
		
		ENDCG
	}
	
	FallBack "Standard"
}
