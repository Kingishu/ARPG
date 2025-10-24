Shader "Hidden/Shader/RadialBlur"
{
	HLSLINCLUDE

#pragma target 4.5
#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

		struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float2 texcoord   : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		return output;
	}

	float _Intensity;//强度
	float _SampleCount;//采样次数
	TEXTURE2D_X(_InputTexture);

	//径向模糊，是一种从中心向外呈幅射状，逐渐模糊的效果。
	//径向模糊（Radial Blur）可以给画面带来很好的速度感

	//径向模糊的原理
	//图形学中模糊的大致原理都是一样的：就是从原像素周围去寻找附近像素的颜色，来共同影响当前的像素颜色。如高斯模糊就是将原像素四周像素的颜色加权求和作为原像素的颜色以达到模糊的目的。
	//不同的模糊就是取周边像素和加权求和的算法不太一样。径向模糊的特点是从某个中心点向外散射扩散，因此其需要采样的像素来自于当前的像素点和中心点的连线上，通过参数可以控制采样的数量和采样步进的距离。像素的颜色是由该像素的点与中心点之间连线上进行采样，然后求将这些采样点颜色的加权平均。根据径向模糊的特性，离目标点越近采样点越密集，反之亦然。
	//因此，实现径向模糊的大致流程如下：
	//确定径向模糊的中心点，一般为画布的中心点，或这个某个对象的中心点在屏幕投影所在的位置。（注意中心点是2维坐标）
	//计算当前像素和中心点的距离和向量线段。
	//在线段上面进行采样，加权。
	//将模糊的结果和原图进行一个叠加合成（可能需要）*/

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		half3 col = 0;
		//-0.5 表示纹理中心朝向当前坐标(input.texcoord)的向量
		half2 symmetryUv = input.texcoord - 0.5;
		//求该向量的长度
		half distance = length(symmetryUv);

		//需要知道在这段距离间隔多远采样一次
		//强度/采样次数*长度   意思是指:当前坐标到屏幕中心的这段距离*()
		half factor = _Intensity / _SampleCount * distance;
		//进行多次采样,并对颜色进行累加
		for (int i = 0; i < _SampleCount; i++) {

			//偏移计算
			half uvOffset = 1 - factor * i;

			//symmetryUv:上面求到的:屏幕朝向当前坐标的方向
			//该方向*偏移+屏幕中心点0.5,则得到屏幕中心点朝该坐标进行一定的偏移量
			half2 uv = symmetryUv * uvOffset + 0.5;

			//屏幕坐标= 计算后偏移的坐标 * 屏幕大小
			uint2 positionSS = uv * _ScreenSize.xy;

			//颜色采样(屏幕输入的纹理,纹理坐标)
			col += LOAD_TEXTURE2D_X(_InputTexture, positionSS).rgb;
		}
		//除以采样次数,就得到平均的颜色 即模糊后的效果
		col /= _SampleCount;
		return float4(col, 1);
	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "RadialBlur"

			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off

			HLSLPROGRAM
				#pragma fragment CustomPostProcess
				#pragma vertex Vert
			ENDHLSL
		}
	}
	Fallback Off
}