🚀 Fragmento Zero

Fragmento Zero é um jogo do gênero auto-runner 2D com estética neon-espacial, desenvolvido como projeto da disciplina de Design e Desenvolvimento de Jogos.
O jogador permanece fixo no lado esquerdo da tela enquanto o mundo se move continuamente para a esquerda. O objetivo é sobreviver pelo maior tempo possível, desviando de obstáculos, utilizando poderes estratégicos e gerenciando sua energia.

🎮 Sobre o Jogo

A principal mecânica de Fragmento Zero é a utilização da energia como recurso central, funcionando simultaneamente como:
Barra de vida;
Combustível para os poderes.
Cada decisão do jogador envolve administrar esse recurso de forma eficiente para alcançar a maior pontuação possível, medida em metros percorridos.

✨ Mecânicas Principais

- Movimentação em quatro faixas;
- Sistema de energia com drenagem contínua;
- Obstáculos dinâmicos (Meteoros e Drones);
- Cristais colecionáveis para recuperação de energia;
- Poder Fogo com sistema de cooldown;
- Poder Gelo com efeito de congelamento temporário;
- Progressão dinâmica de dificuldade;
- Sistema de pontuação baseado em distância percorrida;
- Tela de Game Over e reinício da partida.

🎯 Controles

- ↑ ou Espaço	Subir uma faixa
- ↓	Descer uma faixa
- A	Ativar poder Fogo
- D	Ativar poder Gelo
- R	Reiniciar partida

⚙️ Tecnologias Utilizadas

- Engine: Unity 6.5
- Linguagem: C#
- Controle de versão: Git e GitHub
Bibliotecas:
- UnityEngine
- UnityEngine.UI
- TMPro (TextMeshPro)
- UnityEngine.SceneManagement

🏗️ Arquitetura do Projeto

- Sistema: Script
- Loop principal: GameManager.cs
- Faixas: LaneSystem.cs
- Jogador: PlayerController.cs
- Poder Fogo: PowerFire.cs
- Poder Gelo: PowerIce.cs
- Obstáculos: ObstacleBase.cs
- Meteoro: Meteor.cs
- Drone: Drone.cs
- Cristal: Crystal.cs
- Bala: Bullet.cs
- Spawner: ObstacleSpawner.cs
- HUD: HUDController.cs
- Efeitos:	ScreenEffects.cs

🧪 Playtest

O protótipo passou por sessões de playtest com participantes externos ao grupo, incluindo estudantes universitários, jogadores casuais e adultos entre 25 e 50 anos.
Os testes tiveram duração média entre 10 e 15 minutos e permitiram validar as principais mecânicas do jogo.

Principais resultados
- As regras foram compreendidas rapidamente;
- O sistema de energia foi considerado intuitivo;
- Os poderes apresentaram boa aceitação;
- A interface foi considerada clara;
- O jogo apresentou estabilidade durante os testes.

O feedback recebido indicou oportunidades de melhoria relacionadas à visibilidade dos cristais e ao balanceamento da curva de dificuldade.

▶️ Como Executar o Projeto

Pré-requisitos:
- Unity 6.5 ou superior
- Git

Instalação:
git clone https://github.com/pedroamaral01/jogo-fragmento-zero.git

- Abra o projeto na Unity Hub;
- Selecione a versão Unity 6.5;
- Abra a cena principal em Assets/Scenes;
- Clique em Play.

👥 Desenvolvedores
- Kaiky Linhares Emilio
- Pedro Henrique Amaral Estevão

📚 Projeto Acadêmico

Este projeto foi desenvolvido como atividade da disciplina de Design e Desenvolvimento de Jogos.

🔗 Repositório

https://github.com/pedroamaral01/jogo-fragmento-zero
