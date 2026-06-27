Descrição geral da aplicação:
Sushi Loko é um jogo estilo Escape Room que ocorre em um restaurante de comida japonesa. Para que o jogador complete o jogo, é necessário preparar um Yakisoba utilizando ingredientes e utensílios presentes na cozinha. Após o preparo do prato, o jogador deve entregá-lo ao cliente, que estará esperando em sua mesa.

Mapa do jogo: [Mapa](mapa.png)

Cena 1: Recepção (teleportável)
A1.1 - Porta 1 (grab);
A1.2 - A porta de entrada do restaurante e a porta de acesso à área externa são portas fakes da sala e não responderão a interações (elementos meramente decorativos);
A1.3 - A recepção deve possuir um som ambiente que remeta a uma música japonesa calma e tranquila.

Ideias para a modelagem dos elementos:
- Porta 1 deve ser de metal
- Porta fake da área externa deve ser uma porta de correr e seu material deve ser vidro (deve ser possível enxergar a área externa)
- Balcão deve ser de madeira
- A iluminação da sala deve ser suave e quente


Cena 2: Cozinha (teleportável)
A2.1 - O Painel de Pedidos conterá um botão (BTNP) que deverá ser pressionado para ser ligado (poke);
A2.2 - Ao ligar o painel, deve ser exibido um pedido com o prato que o jogador deverá preparar e a mesa solicitante. Neste caso, o prato a ser preparado será um Yakisoba;
A2.3 - A Porta 2 é a porta da geladeira;
A2.4 - Dentro da geladeira deve existir um recipiente com brócolis (B), que pode ser pego pelo jogador (grab);
A2.5 - PN é a panela onde o yakisoba deve ser preparado;
A2.6 - Ao iniciar o jogo, PN já deve estar preenchida com parte do yakisoba, restando apenas adicionar o brócolis;
A2.7 - O fogão possui um botão (BTNF) que deve ser apertado para cozinhar o yakisoba (poke);
A2.8 - Após 10 segundos no fogo, o yakisoba fica pronto;
A2.9 - Depois de cozinhar, o jogador deve poder pegar a panela (grab);
A2.10 - O yakisoba de PN deve ser colocado em PR1 (grab) para habilitar o teletransporte para a área externa;
A2.11 - Porta 2 (grab);
A2.12 - Ao liberar o teletransporte para a área externa, a Porta 2 deve apresentar um outline;
A2.13 - O som ambiente da cozinha deve ser composto pelo barulho de máquinas e utensílios de cozinha em operação.

Ideias para a modelagem dos elementos:
- Painel de pedidos deverá ser similar a uma televisão que exibe os pedidos que chegam à cozinha
- A pia deve conter louças japonesas
- A bancada deve conter um par de hashis


Cena 3: Área Externa (teleportável)
A3.1 - PR1 deverá ser levado e colocado sobre a mesa 2 para finalizar o jogo (grab);
A3.2 - Quando PR1 é colocado sobre a mesa 2, é exibida a mensagem "Fim de Jogo" e a interação é encerrada;
A3.3 - O som ambiente da área externa deve ser composto por uma música japonesa tranquila diferente da recepção, acompanhada de vozes ao fundo conversando.

Ideias para a modelagem dos elementos:
- Ambiente externo deve conter iluminação suave
- As mesas devem estar sobre um deck de madeira
- A área externa deve estar rodeada por árvores cerejeiras floridas
- Deve conter um caminho de pedras conectando o deck das mesas à porta fake que conecta a recepção
