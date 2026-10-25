extends RefCounted



static func fraction(rate_per_min: float, minutes: float) -> float:
    return 1.0 - exp( - maxf(0.0, rate_per_min) * maxf(0.0, minutes))


static func chain(a: float, b: float, k1: float, k2: float, minutes: float) -> Array:
    var ea = exp( - k1 * minutes)
    var eb = exp( - k2 * minutes)
    var formed: float
    if absf(k1 - k2) < 1e-10:
        formed = k1 * a * minutes * ea
    else:
        formed = k1 * a * (ea - eb) / (k2 - k1)
    return [a * ea, b * eb + formed]


static func exchanger_effectiveness(ua: float, hot_capacity: float, cold_capacity: float) -> float:
    var cmin = minf(hot_capacity, cold_capacity)
    var cmax = maxf(hot_capacity, cold_capacity)
    if cmin <= 1e-09 or ua <= 0.0: return 0.0
    var cr = cmin / cmax
    var ntu = ua / cmin
    if absf(1.0 - cr) < 1e-07: return ntu / (1.0 + ntu)
    var x = exp( - ntu * (1.0 - cr))
    return clampf((1.0 - x) / (1.0 - cr * x), 0.0, 1.0)

static func pipe_capacity(length_m: float, bore_mm: float) -> float:
    return PI * pow(bore_mm / 1000.0, 2.0) / 4.0 * length_m * 1000.0


static func exchange_bodies(t1: float, c1: float, t2: float, c2: float, ua: float, seconds: float) -> Array:
    if c1 <= 1e-09 or c2 <= 1e-09: return [t1, t2]
    var equilibrium = (t1 * c1 + t2 * c2) / (c1 + c2)
    var decay = exp( - maxf(0.0, ua) * (1.0 / c1 + 1.0 / c2) * seconds)
    return [equilibrium + (t1 - equilibrium) * decay, equilibrium + (t2 - equilibrium) * decay]


static func pipe_temperature(temperature: float, ambient: float, u_w_m2k: float, 
        length_m: float, bore_mm: float, heat_capacity_kj_k: float, seconds: float) -> float:
    if heat_capacity_kj_k <= 1e-09: return temperature
    var ua = u_w_m2k * PI * bore_mm / 1000.0 * length_m / 1000.0
    return ambient + (temperature - ambient) * exp( - ua * seconds / heat_capacity_kj_k)
