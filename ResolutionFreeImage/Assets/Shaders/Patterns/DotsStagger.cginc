#ifndef RFI_PATTERN_DOT_STAGGER_CGINC
#define RFI_PATTERN_DOT_STAGGER_CGINC

// ## Calculate EXACT distance as possible as you can in pattern functions. ##

#include "ShapeUtils.cginc"

fixed4 dotStaggerPattern(float2 uv, float es, float diameter, fixed4 col0, fixed4 col1) {
    float2 p = abs(frac(uv) * 2.0 - 1.0);
    float r = diameter;// * 0.5 * 2.0
    float d = (min(length(p), length(p - float2(1.0, 1.0))) - r) * 0.5;

    // vec3 rgb = vec3(0.0);
    // rgb.xy = p;
    // rgb.xy = ap;
    // rgb = vec3(-d, d, exp(-d*d*10000.0));
    return lerp(col0, col1, edgeWeight(d, es));
}

#endif
