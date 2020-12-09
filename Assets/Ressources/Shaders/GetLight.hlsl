#ifndef GETLIGHT_INCLUDED
#define GETLIGHT_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightDefinition.cs.hlsl"

void GetSun_float(in float rotationSeason, out float3 lightDir, out float3 color) {
#if SHADERGRAPH_PREVIEW
	lightDir = float3(0.6, 0.6, 0.0);
	color = 1;
#else
	if (_DirectionalLightCount > 0) {
		DirectionalLightData light = _DirectionalLightDatas[0];
		light.forward.y += rotationSeason * 23.44 * 3.14159265f / 180; // adjust Earth rotation for winter
		lightDir = -light.forward.xyz;
		color = light.color;
	}
	else {
		lightDir = float3(1.0, 0.0, 1.0);
		color = 0;
	}
#endif
}
#endif

