DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

Nesse tutorial vamos continuar adicionando coisas no cenário, usar um componente que controla a animação de assets (reproduzindo o efeito de uma dobradiça), além de rever o Near-Far Interactor (do último tutorial), explorar outro tipo de interação e executar a locomoção mais sugerida para evitar o enjoo que é o teleporte.. 

Este documento tem uma duração estimada de 4 horas/aula, ou seja 3 horas e 15 minutos, aproximadamente. Execute o tutorial com calma e tranquilidade. Tenha certeza de que está cumprindo com todos os passos. 

## **2.1.2 Portas** 

Inicie baixando do Moodle o arquivo minhaporta.zip e descompacte para a pasta Assets. Como não encontrei uma porta que funcionasse para RV, eu construí uma porta juntando cubos. Não é bonita, mas pode servir para te inspirar no próximo trabalho se não encontrar o asset que precisa. 

No último tutorial não foi comentado onde posicionar as mesas na sala. Se você colocou no meio da sala, selecione todos os objetos na guia hierarquia e arraste para um canto da sala. Faça o mesmo com seu personagem (XR Origin). Nós vamos dividir essa sala em duas parte permitindo um acesso por uma porta. Adicione uma porta, a partir da pasta MinhaPorta>Prefabs. Adicione no cenário, quase no centro da sala. Talvez tenha que rotacionar no eixo Y em 90. Utilize os assets Wall que você já conheceu na aula anterior para criar as paredes nas laterais da porta. E não esqueça de colocar uma Wall em cima da porta. Abaixo tem duas imagens que vão te dar uma ideia do que precisa ser feito. Não esqueça de atribuir o layer Obstaculos para as duas paredes e na porta, no objeto interno Espelho. Quando mudar o layer dele, o Unity vai abrir uma janela perguntando se queres atribuir nos filhos, e selecione [No, this object only]. 

Agora vamos nos concentrar na porta. Se você esse objeto na guia [Hierarchy], observará que ela está dividida entre Espelho e Caixilho, ainda dentro de Espelho tem os dois trincos. No primeiro trabalho vais precisar limitar a porta se abrir só depois do jogador resolver um problema. Para isso, vais criar um script e atribuir para a Porta solicitada no primeiro trabalho. 

Agora vamos fazer para abrir e fechar a porta como um objeto interagível. Adicione o componente XR Grab Interactable em Espelho. Em Rigidbody>Collision Detection mude para Continuous Dynamic e em XR Grab Interactable>Movement Type mude para Velocity Tracking. Esses dois ajustes, que você realizou na aula passada, servem para melhorar a qualidade das detecções com as colisões e atualizações de tela. Ainda em XR Grab Interactable, abra a lista Colliders, clique em [+] e arraste Trinco1 para ele. Execute e vamos ao primeiro teste da porta usando a tecla G. Você vai conseguir mover a porta livremente como fazia com o tubo de ensaio. Mas não conseguimos as mesmas técnicas que foram usadas para limitar a saída do tubo no rack. Vamos usar os Joints, que é um tipo de componente que você utilizará sempre que for implementar uma gaveta, uma janela, ou uma porta. Elas servem para limitar esse movimento com Grab. A porte de correr usa um tipo de Joint diferente dessa de abrir. 

Selecione Espelho e adicione o componente Hinge Joint. Esse é um componente que limita o grab de forma a usar os ângulos, como se fosse uma dobradiça. Clique no botão Edit Angular Limits e no editor vai apresentar um círculo laranja na porta indicando o ponto de virada do objeto (a princípio ele mostra num canto superior). 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

Vamos alterar para que âncora fique no meio da porta X=0 Y=0.05 e Z=-0.45 e que para que vire só no eixo Y vais alterar Anchor para X=0 Y=1 e Z=0 (acompanhe o círculo laranja à medida que vai alterando essas configurações). Agora vamos configurar os limites de abertura e fechamento. Clique novamente em Edit Angular Limits e no editor observará que a ferramenta muda de aparência. Agora vamos configurar o ângulo de abertura da porta. Clique em Use Limits. Mais abaixo, abra o atributo Limits e digite Min=-120 e Max=1. 

Deixe desmarcado Use Spring, e abra o atributo Spring. Eu vou te mostrar para que serve, pois poderás usar isso no próximo trabalho. Spring > Spring é a força da mola para configurar um fechamento automático da porta. Spring > Damper refere-se à suavização da porta quando ela estiver fechando. 

A porta pode colidir com os objetos dos caixilhos e o seu comportamento ficará muito estranho. Então, precisamos configurar para que os layers a que pertencem os caixilhos não interajam com o layer do Espelho da Porta. Para isso, vá em Edit>Project Settings>Physics>Settings>Layer Collision Matrix e desmarque a caixa da linha Default com Obstaculos. 

Teste e verá que o comportamento da porta conforme o que precisávamos. Depende o ângulo que você tentar abrir, ela terá um comportamento esquisito. 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S. Para compilar as alterações no código, também basta salvar o script. 

## **2.2. Poke Interactor** 

Essa é uma interação de toque. O controle, ou a mão, devem tocar nos objetos interagíveis. Para treinar um pouco esse tipo de interação, vamos exercitar pressionando botões. Esse seria outro caso como da porta, em que é mais fácil criarmos o botão com nosso próprio design. Mas vamos usar algo pronto que já está no projeto. 

Vamos começar providenciando a bancada dos botões. Na segunda sala do cenário, crie uma mesa com dois “shelf showcase” e um “shelf” e disponha dessa forma como ilustrado na imagem abaixo. Arraste o XR Origin para ficar de frente a essa bancada, para facilitar os testes, e não precisar todo momento abrir a porta. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

Sobre essa mesa disponha três botões (Samples> XR Interaction Toolkit>XX.XX.XX> Starter Assets> DemoSceneAssets> Prefabs> Interactables > Push Button). Se por acaso, você está usando uma versão mais antiga, vá para a raiz do projeto (pasta Assets) e no campo de pesquisar, digite Push. Nos resultados, ao clicar sobre item, veja se ele é um Prefab. 

Quando adicionar o primeiro, altere a sua escala para 2 (X, Y e Z) e duplique para os outros dois botões. Na pasta Materials crie três materiais que serão usados para colorir os botões. Em cada Push Button, na Hierarchy, abra os elementos filhos até alcançar EmergencyStop. Na guia Inspector, no componente MeshRenderer, arraste um desses novos materiais para Materials > 0. 

Vai ficar mais ou menos assim a disposição desses botões na bancada. Teste o jogo. Agora experimente arrastar o controle da direita para cima do botão, e mude no eixo Y para ver o botão ser pressionado. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

Vamos explorar o tratamento de eventos ao pressionar em um botão. Vamos fazer para um deles, e depois você pode replicar nos demais. Na guia Inspector do elemento PushButton, adicione um componente BotaoPressionado que será um script. Procure na pasta Assets e arraste esse asset para dentro de Scripts. Abra ele no seu editor, e digite o seguinte código. 

`using UnityEngine; using UnityEngine.XR.Interaction.Toolkit.Interactables; public class BotaoPressionado : MonoBehaviour { public int numBotao; void Start() { GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x=>Pressionei()); } public void Pressionei() { // aqui vai a sua logica de acordo com o puzzle print("PRESS " + numBotao); } }` 

Observe que quando o botão for pressionado, imprimirá na console um texto informando o número que foi pressionado. Logo, na guia Inspector, na área desse componente, preencha um número de 1 a 3 no atributo Num Botao. Adicione esse componente BotaoPressionado nos outros dois botões, não esqueça de alterar o atributo Num Botao. Execute o jogo testando o pressionamento dos três botões e observará na console os textos impressos conforme a ordem que você adotou. 

Ainda temos que resolver um erro: quando você usa o Grab está acionando o pressionar dos botões. Selecione o elemento PushButton na hierarquia, e na inspector, no componente XR Simple Interactable existe um atributo Interaction Layer Mask. Clicando nele crie um novo layer chamado PokeOnly. Retorne nesse atributo, marque PokeOnly e desmarque Default. Faça o mesmo com os outros dois PushButtons. Selecione o elemento XR Origin>Camera Offset>Main Camera>Right Controller>Poke Interactor e no atributo Interaction Layer Mask faça o mesmo procedimento que foi realizado com os Push Buttons. Repita esse processo com o Left Controller. 

Execute e vai reparar que melhorou bastante o comportamento dos controles em relação aos botões. 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S. Para compilar as alterações no código, também basta salvar o script. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

## **3. Teleporte** 

Quando você usa no simulador a tecla I está usando o direcional para frente do controle direito, que está configurado para ação de teleporte. Porém, a linha está em vermelho indicando que não existe ainda uma área para acontecer o teleporte. 

Vamos primeiro explorar essa funcionalidade das linhas. Observe em [Hierarchy] > [XR Origin (XR Rig)] > [Camera Offset] > [Right Controller] > [Teleport Interactor] e na guia [Inspector] role para baixo e observe os componentes Line Renderer e XR Interactor Line Visual que estabelecem algumas cores a serem usadas no uso do teleporte e nos dois últimos atributos desse componente ([Reticle] e [Blocked Reticle]) podes explorar as figuras que aparecem indicando a possibilidade, ou não, de teleporte. 

Vamos começar criando na hierarquia um elemento vazio chamado Chaos. Coloque o Chao como filho e renomeie ele para Chao1 e altere o tamanho dele para ficar somente como chão da sala 1. Selecione o Chao1 em [Hierarchy] e em [Inspector] adicione o componente [Teleportation Area]. Arraste Mesh Collider para Colliders de Teleportation Area. No campo Interaction Layer Mask garanta que só o valor Teleport esteja marcado. Copie e cole esse Chao1 duas vezes, renomeando para Chao2 e Chao3, e altere as suas posições e os seus tamanhos, para que o Chao2 torne segura a sala 2 e o Chao3 torne seguro o chão fora do laboratório. 

Na guia Inspector, desabilite do Chao2 e Chao3 o componente Teleportation Area. 

Mude XROrigin novamente de posição para a sala 1. Agora você pode executar e use a tecla I. Verá que sai uma linha do controle: vermelho para as áreas inválidas (quando você apontar para fora da sala) e azul para a área do teleporte. Quando estiver azul, solte a tecla I para teleportar. 

Vamos desenvolver e utilizar uma classe que habilita ou desabilita a área de teleporte da sala 2 quando abrimos ou fechamos a porta. Inicie criando um script na pasta Scripts chamado EventosPorta. 

`using UnityEngine; using UnityEngine.Events; using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation; public class EventosPorta : MonoBehaviour { private bool isOpen = false; private HingeJoint hinge; public TeleportationArea teleporte; void Start() { hinge = GetComponent<HingeJoint>();` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

`} void Update() { float angle = hinge.angle; // abriu if (!isOpen && angle == -120) { isOpen = true; teleporte.enabled = true; } else { // Porta fechou // por causa da precisao do float, tive que testar com 1 grau a menos if (isOpen && angle > -119) { isOpen = false; teleporte.enabled = false; } } } }` 

Na Hierarquia, selecione o elemento Espelho e adicione o componente EventosPorta. Arraste o Chao2 para o atributo Teleporte. 

Você pode se inspirar nesse código para fazer a gestão de puzzle da sala 1 para habilitar a abrir essa porta. 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S. Para compilar as alterações no código, também basta salvar o script. 

