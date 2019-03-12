#ifndef RFI_PATTERN_CHECKER_CGINC
#define RFI_PATTERN_CHECKER_CGINC

// ## Calculate EXACT distance as possible as you can in pattern functions. ##

// #include "ShapeUtils.cginc"

float checkerDistance(float2 uv) {
    float2 p = frac(uv) * 2.0 - 1.0;
    // p = fract(p);

    float2 ap = abs(abs(p) - 0.5);
    float d = 0.5 - max(ap.x, ap.y);
    d *= sign(p.x) * sign(p.y);
    return d * 0.5;
}

fixed4 checkerPattern(float2 uv, float es, fixed4 col0, fixed4 col1) {
    float d = checkerDistance(uv);
    // rgb.xy = p;
    // rgb.xy = ap;
    // rgb = float3(-d, d, exp(-d*d*10000.0));
    return lerp(col0, col1, edgeWeight(d, es));
}

#endif