import random
import math

# Paramètres de la couche
fan_in = 3      # Exemple : image 28x28
fan_out = 3     # Nombre de neurones

# Limite de Xavier
limit = math.sqrt(6 / (fan_in + fan_out))

# Génération des poids
weights = [
    [random.uniform(-limit, limit) for _ in range(fan_in)]
    for _ in range(fan_out)
]

# Affichage
for neuron_index, neuron_weights in enumerate(weights):
    print(f"Neurone {neuron_index}:")
    print(neuron_weights)
    print()