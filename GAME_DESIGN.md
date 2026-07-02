# Fragmento Zero — Demo: Modo Fenda Infinita (v2.0)

Auto-runner 2D neon-espacial. O **player fica parado** no lado esquerdo (X = -5) enquanto o **mundo se move para a esquerda**. Score medido em metros percorridos. A demo cobre o loop completo: menu → corrida → evolução → chefões → game over → recorde.

---

## Fluxo do jogo (GameState)

```
Menu ──ENTER──▶ Running ◀──────┐
  ▲                │            │
  │             (evolução)      │ vitória: +1 tier,
  │                ▼            │ +35 energia, +200 m
  │            BossFight ───────┘
  │                │
  └──[M]── GameOver ◀── energia = 0        Paused: ESC alterna (timeScale 0)
```

- **Classificação indicativa** (ClassInd 10 / ESRB E10+) exibida antes do menu, 1× por sessão.
- **Restart rápido**: R após a morte pula o menu (autoStartRun).
- **Tutorial** contextual na primeira corrida (faixas, cristais, fogo) — 1× por instalação (PlayerPrefs).

## Player — Energia como Vida e Recurso

| Parâmetro | Valor (GameConfig) |
|---|---|
| Energia inicial / máxima | 60 / 100 |
| Drenagem passiva | 1.2 / s |
| Dano por obstáculo/projétil | -25 |
| Invencibilidade pós-hit | 1.17 s (pisca) |
| Recompensa por kill | +6 |

Energia ≤ 0 → Game Over. Barra fica vermelha abaixo de 25%.

## Movimentação — 4 Faixas

↑ / Espaço sobe, ↓ desce; interpolação suave (Lerp ×10).

## Poderes (PowerBase)

Desbloqueados pela **evolução**, na ordem:

| Poder | Tecla | Custo | Cooldown | Efeito |
|---|---|---|---|---|
| **Fogo** | A | 6 de **carga** | 0.37s | Projétil (1 dano). Meteoro morre com 1, drone com 2 |
| **Gelo** | D | 22 energia | — (4s de efeito) | Meteoros/cristais a 25%, drones parados, projéteis de boss a 35% |
| **Raio** | S | 25 energia | 6s | Cadeia em até 4 inimigos visíveis (2 dano) — limpa hordas |
| **Gravidade** | F | 18 energia | 8s | Por 5s cristais são puxados ao player |

**Carga de Fogo**: recurso próprio do Fogo (0–100). Regenera 4/s durante o gameplay e é restaurada (+30) pelos **cristais de fogo laranja**. Na HUD, a barra do Fogo mostra a carga descendo a cada tiro e subindo com regen/cristais.

**Cristais**: azuis restauram energia/vida (+18); laranja restauram só a carga de Fogo (+30). Na **arena de chefão** o suporte spawna cristal de fogo a cada ~4s e cristal azul raramente (~15s) — atirar é abundante, curar é escasso.

## Evolução do Fragmento (EvolutionSystem)

Acumula **toda energia absorvida** (cristais +18, kills +6):

| Nv | Estágio | Marco | Ganho |
|---|---|---|---|
| 0 | Centelha | — | Fogo |
| 1 | Fragmento Desperto | 60 | Gelo + **mini-chefão** |
| 2 | Núcleo Instável | 150 | Raio + **Guardião de Magma** |
| 3 | Avatar Elemental | 280 | Gravidade + chefes recorrentes |

Cada estágio muda cor, escala e trail do Fragmento (toast anuncia).

## Geração Procedural (SpawnPattern + ObstacleSpawner)

Padrões declarativos por colunas/faixa (`"MM.M"` = parede com brecha; M meteoro, D drone, C cristal). 14 padrões com peso, espelhamento vertical e tier mínimo — paredes, corredores, diagonais de cristais, esquadrões. Um multiplicador de velocidade por padrão mantém formações coesas.

## Dificuldade (DifficultyDirector)

Tier sobe por distância (250/600/1000/1500/2100/2800 m) **e** +1 permanente por chefão vencido. Cada tier: padrões mais agressivos, spawns ~12% mais densos, +0.4 de velocidade. Velocidade tem teto (11).

## Chefões (BossBase / BossDirector)

Construtos da Mente Matriz; spawner pausa durante a luta; entrada invulnerável; fases por fração de HP; barra de vida no topo. HP escala +35% por encontro.

| Chefe | HP base | Fases | Padrões |
|---|---|---|---|
| **Drone Alfa** (mini) | 14 | 2 | Persegue faixa, tiro mirado; enraivecido: leque triplo |
| **Guardião de Magma** | 26 | 3 | Senoide, leques de magma 3→4→5, arremesso de meteoros; **Gelo contém o magma** |

Vitória: +1 tier, +35 energia, +200 m; próximo encontro a ~650 m.

## Áudio e VFX

100% procedurais (sem assets): SFX sintetizados (SfxSynth) e explosões de fragmentos (BurstVFX), ambos dirigidos por eventos. Música: drone ambiente em loop.

---

## Arquitetura Técnica (Unity 6.5)

| Camada | Peças | Papel |
|---|---|---|
| **Core** | `GameManager` (estados), `GameEvents` (Observer), `GameConfig` (SO de balanceamento), `DifficultyDirector`, `HighScore`, `LaneSystem` | Estado global, eventos discretos, números centralizados |
| **Player** | `PlayerController`, `PowerBase` + 4 poderes, `EvolutionSystem`, `Bullet` | Input genérico por poder; energia; evolução |
| **World** | `ObstacleBase` (+Meteor/Drone), `Crystal`, `SpawnPattern`, `ObstacleSpawner` | Entidades anunciam eventos; spawn por padrões |
| **Bosses** | `BossBase`, `BossProjectile`, `BossDirector`, 2 chefes | Fases, ciclo corrida→chefe→corrida |
| **UI** | `UiFactory` + controllers (Menu/Pause/GameOver/Toast/Tutorial/BossBar/HUD) | UI construída em runtime, reativa a eventos |
| **VFX/Audio** | `ScreenEffects`, `BurstVFX`, `LightningBoltVFX`, `RuntimeSprites`, `SfxSynth`, `AudioManager` | Feedback 100% por eventos, zero assets |

**Princípios:**
- Entidades **anunciam** (`GameEvents.Raise…`); consequências (energia, score, som, VFX, UI) são dos assinantes — OnEnable/OnDisable sempre pareados.
- Valores contínuos (energia/score) são lidos por polling com refs cacheadas; eventos só para fatos discretos.
- `IDamageable`/`IPlayerHazard` desacoplam combate de hierarquias concretas.
- Bootstraps (`UIBootstrap`, `GameSystemsBootstrap`) criam raízes persistentes (DontDestroyOnLoad) — a cena contém só o essencial (player, spawner, HUD base, câmera).

**Regra física crítica:** objetos com trigger (`OnTriggerEnter2D`) precisam de `Rigidbody2D Kinematic`.

**Editor tooling:** `FragmentoZero/Setup Scene`, `Create Obstacle Prefabs`, `Create Game Config Asset`, `Build Game (Windows)`.
