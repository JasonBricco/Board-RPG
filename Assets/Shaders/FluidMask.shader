Shader "Custom/Fluid Mask" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (RGB)", 2D) = "white" {}
		_Alpha("Base Alpha", float) = 0.5
		_Speed("Base Speed", float) = 2.0
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
        	float2 uv2_MaskTex;
        	float4 color : COLOR;
      	};
      	
      	sampler2D _MainTex;
      	sampler2D _MaskTex;
      	float _Alpha;
      	float _Speed;
      	
      	void vert(inout appdata_full v) 
      	{
        	v.texcoord.x += _Time.y * _Speed;
      	}
		
      	void surf(Input IN, inout SurfaceOutput o) 
      	{
      		half4 main = tex2D(_MainTex, IN.uv_MainTex);
      		half4 mask = tex2D(_MaskTex, IN.uv2_MaskTex);

      		half4 res = mask;
      		float alpha = _Alpha;

      		if (mask.a < 0.1)
      			res = main;
      		else 
      		{
      			res = mask;
      			alpha = 1.0;
      		}

      		o.Albedo = res;
      		o.Alpha = alpha;
      	
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
