#ifndef RFI_SHAPEFUNCS_CGINC
#define RFI_SHAPEFUNCS_CGINC

// Rect
float distanceFromRect(float2 p, float2 s) {
    float2 hs = s * 0.5;
    float2 cp = hs - abs(p - hs);
    return min(cp.x, cp.y);
}
   
// Capsule
 float distanceFromCapsule(float2 p, float2 s) {
    float2 hs = s * 0.5;
    float2 ap = abs(p - hs);
    float2 cp = (hs.x < hs.y)? float2(0.0, hs.y - hs.x) : float2(hs.x - hs.y, 0.0);
    float d = length(max(float2(0.0, 0.0), ap - cp));
    return min(hs.x, hs.y) - d;
 }

// Round Rect
float distanceFromRoundRect(float2 p, float2 s, float r) {
    float2 hs = s * 0.5;
    float2 ap = abs(p - hs);
    float2 cp = float2(hs.x - r, hs.y - r);
    float2 vac = ap - cp;

    float d0 = min(0.0, max(vac.x, vac.y));
    float d1 = length(max(float2(0.0, 0.0), vac));

    return r - (d0 + d1);
}

// Trimed Rect
float distanceFromTrimedRect(float2 p, float2 s, float r) {
    float2 hs = s * 0.5;
    float2 ap = abs(p - hs);
    float2 rp = hs - ap;
    float td = dot(ap - (hs - float2(r, 0.0)), float2(-0.707106781, -0.707106781));
    return min(td, min(rp.x, rp.y));
}

#endif
