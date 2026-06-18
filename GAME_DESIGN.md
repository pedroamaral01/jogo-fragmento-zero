# Fragmento Zero — Lógica e Regras do Jogo (v1.0)

Auto-runner 2D neon-espacial. O **player fica parado** no lado esquerdo (X = -5) enquanto o **mundo se move para a esquerda**. Score medido em metros percorridos.

---

## Player — Energia como Vida e Recurso

A energia funciona ao mesmo tempo como **barra de vida** e **combustível dos poderes**.

| Parâmetro | Valor |
|---|---|
| Energia inicial | 60 / 100 |
| Drenagem passiva | ~1.2 / segundo (constante) |
| Dano por obstáculo | -25 |
| Invencibilidade pós-hit | 1.17s (player pisca) |

- Energia ≤ 0 → **Game Over**
- Energia < 25% → barra fica **vermelha**

---

## Movimentação — 4 Faixas

```
Faixa 0  (topo)
Faixa 1
Faixa 2  ← posição inicial
Faixa 3  (baixo)
```

| Tecla | Ação |
|---|---|
| ↑ / Espaço | Sobe uma faixa |
| ↓ | Desce uma faixa |

Movimento suavizado com `Mathf.Lerp` (fator 10) — não é instantâneo.

---

## Obstáculos

Todos spawnam à direita da tela e se movem para a esquerda.
**Velocidade** = `GameManager.Speed × SpeedMultiplier (1.0–1.43× aleatório por instância)`

| Obstáculo | Cor | Collider | HP | Efeito no player |
|---|---|---|---|---|
| **Meteoro** | Laranja (círculo) | CircleCollider2D | 1 | -25 energia |
| **Drone** | Magenta + olho vermelho | BoxCollider2D | 2 | -25 energia |
| **Cristal** | Ciano (diamante 45°) | CircleCollider2D | — | +energia (coletável) |

- Obstáculos com tag `"Obstacle"` → colisão com player chama `OnHitPlayer()` → `-25 energia + invencibilidade`
- Cristais com tag `"Crystal"` → colisão com player chama `OnCollected()` → `+energia`
- Saem em X < -10 → destruídos automaticamente
- Matar obstáculo com bala → **+6 energia** de recompensa

---

## Poderes

### Fogo — tecla `A`

Dispara uma **bala amarela** para a direita.

| Parâmetro | Valor |
|---|---|
| Custo de energia | 4 por tiro |
| Cooldown | 0.37s |
| Velocidade da bala | 20 u/s |
| Lifetime da bala | 1.17s |
| Dano | 1 HP por acerto |

- Bala acerta tag `"Obstacle"` → `TakeDamage(1)`
- Meteoro morre com 1 tiro, Drone precisa de 2 tiros
- Matar obstáculo → **+6 energia** ao player
- Barra **laranja** no HUD mostra recuperação do cooldown (vazia → cheia em 0.37s)

### Gelo — tecla `D`

Ativa modo **freeze** por 4 segundos.

| Parâmetro | Valor |
|---|---|
| Custo de energia | 22 |
| Duração | 4s |
| Efeito em Meteoros | 25% da velocidade normal |
| Efeito em Drones | Parada total (0%) |
| Efeito em Cristais | 25% da velocidade normal |

- Barra **ciana** no HUD mostra contagem regressiva em segundos
- Só pode usar com energia ≥ 22 e cooldown = 0

---

## Progressão de Dificuldade

`GameManager` aumenta a velocidade do jogo continuamente. Quanto mais metros, mais rápido os obstáculos chegam — o jogo nunca termina por limite de tempo, só por energia zerada.

---

## Game Over e Reinício

| Condição | Resultado |
|---|---|
| Energia ≤ 0 | Game Over — pausa spawning, exibe painel com score em metros |
| Tecla `R` | Reinicia (`SceneManager.LoadScene`) — tudo volta ao estado inicial |

---

## HUD

```
[canto superior esquerdo]  Score: 0 m
[canto inferior esquerdo]  ████████████  ← Barra de energia (ciano → vermelho < 25%)
                           A FOGO ►  [██░░░]  ← Cooldown (laranja)
                           D GELO    [██████]  ← Duração freeze (ciano)
```

---

## Estratégia Básica

- **Energia** é o recurso central — toda decisão envolve gastar ou recuperar energia
- Usar Fogo custa 4, mas matar o obstáculo devolve 6 → **lucro de +2** se acertar
- Usar Gelo custa 22 — vale a pena com muitos obstáculos na tela ao mesmo tempo
- Deixar Drone chegar custa 25 de energia; 2 tiros de Fogo custam 8 → **sempre vale atirar no Drone**
- Coletar Cristais recarrega energia — **priorize a faixa com cristais** quando a energia estiver baixa

---

## Arquitetura Técnica (Unity 6.5)

| Sistema | Script | Função |
|---|---|---|
| Loop do jogo | `GameManager.cs` | Estado (Playing/Dead), velocidade, score |
| Faixas | `LaneSystem.cs` | Calcula posição Y de cada faixa |
| Player | `PlayerController.cs` | Input, energia, colisões, invencibilidade |
| Poder Fogo | `PowerFire.cs` | Spawn de bala, cooldown, custo |
| Poder Gelo | `PowerIce.cs` | Timer de freeze, SlowFactor |
| Obstáculos | `ObstacleBase.cs` | Movimento, HP, dano |
| Meteoro | `Meteor.cs` | SlowFactor via PowerIce |
| Drone | `Drone.cs` | Para completamente com Gelo |
| Cristal | `Crystal.cs` | Recompensa de energia |
| Bala | `Bullet.cs` | Movimento, colisão com obstáculos |
| Spawner | `ObstacleSpawner.cs` | Gera obstáculos aleatoriamente |
| HUD | `HUDController.cs` | Barra energia, score, barras de poder |
| Efeitos | `ScreenEffects.cs` | Screen shake, flash ao acertar |

**Regra física crítica:** todos os objetos com trigger (`OnTriggerEnter2D`) precisam de `Rigidbody2D Kinematic` — Player e todos os prefabs têm este componente.
