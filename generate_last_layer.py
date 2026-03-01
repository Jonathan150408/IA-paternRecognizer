import random
import math

fan_in = 200
fan_out = 3

limit = math.sqrt(6 / (fan_in + fan_out))

print("linear+")
for _ in range(3):
    weights = [str(round(random.uniform(-limit, limit), 6)) for _ in range(200)]
    weights.append("0")  # bias
    print(" ".join(weights) + ";")