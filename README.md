# Reinforcement Learning - DW

Zabawa w epidemicznego berka - coś podobnego do chowanego OpenAI:

[https://youtu.be/Lu56xVlZ40M](https://youtu.be/Lu56xVlZ40M)

### Porównanie

- jak zachowują się agenci po zmianie taktyki; czy współpracują, jak zmienia się strategia,
- jak środowisko wpływa na zachowanie agentów (dodanie ścian itd.)
- zwiększanie szybkości nie-berków

### Obserwacje

- raycasting
- rotation
- velocity

### Rewards

- -0.1 for tagger for every step and +0.1 for others,
- distance to other agents

### Questions

- Czy umieszczać agentów w konkretnych pozycjach czy losowych?
- Czy definiujemy stałą liczbę agentów?
- W jaki sposób agent zostaje uznany za złapanego i staje się berkiem? Np. przy dotknięciu w dowolnym miejscu czy dotknięcie w określonym obszarze.
- Czy umieszczamy w późniejszym etapie przeszkody? Czy dajemy możliwość np. schowania się przed berkami?
- Czy jeśli umożliwimy chowanie się, to czy dopuszczamy możliwość, że ktoś nie zostanie złapany.

## Interesting papers

[Multi-Agent Game Abstraction via Graph Attention Neural Network](https://arxiv.org/abs/1911.10715)

## Co zamierzamy dalej?

- Poprawa formy wizualnej na trochę bardziej przyjemną dla oka
- Dodanie przeszkód 
- Zmiana prędkości - obiekty uciekające mogłyby poruszać się szybciej (albo od początku albo gdy maleje ich liczba)
- Bardziej "przemyślany" ruch, uwzględnienie rotacji (można przemyśleć, czy berek może złapać uciekającego tylko, gdy jest do niego skierowany w odpowiednią stronę czy jakkolwiek go nie dotknie)
- Raycasting
- Przemyślenie systemu nagród i wprowadzenie jego lepszej, bardziej sensownej wersji
- Analiza taktyki (np. współpraca czy niezależenie od siebie)
