Shader "Custom/Alpha Diffuse" 
{
	Properties 
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull off
		
		CGPROGRAM
      	#pragma surface surf Lambert noambient exclude_path:deferred exclude_path:prepass noshadow nolightmap nodynlightmap nodirlightmap nometa noforwardadd
      	
		struct Input 
		{
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
      	
		sampler2D _MainTex;
		
      	void surf(Input IN, inout SurfaceOutput o) 
      	{
      		fixed4 color = tex2D(_MainTex, IN.uv_MainTex);
        	o.Albedo = color.rgb;
        	o.Alpha = color.a;
			clip(color.a - 0.01f);
			
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