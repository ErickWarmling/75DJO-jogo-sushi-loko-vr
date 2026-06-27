DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

Nesse tutorial vamos continuar adicionando mais uma porta no cenário, mas agora de correr usando Near Far Interactor e conhecendo mais um tipo de limitação (parecido com o Hinge Joint). Além disso, vamos conhecer como compilar e disponibilizar o nosso jogo para o Oculus Quest. 

Este documento tem uma duração estimada de 4 horas/aula, ou seja 3 horas e 15 minutos, aproximadamente. Execute o tutorial com calma e tranquilidade. Tenha certeza de que está cumprindo com todos os passos. 

## **2.3. Correção: Grab no Trinco da Porta** 

Vamos corrigir um comportamento que foi modificado com o Unity 6.0. 

Você deve ter observado que mesmo atribuindo os Colliders do Espelho para o trinco, o grab funcionava apontando para o espelho da porta e não para o trinco. Então essa vai ser a primeira correção. 

No elemento Espelho remova XR Grab Interactable. No Trinco01 adicione XR Grab Interactable. Em Rigidbody>Collision Detection mude para Continuous Dynamic e em XR Grab Interactable>Movement Type mude para Velocity Tracking. Adicione o componente Fixed Joint e arraste Espelho para o atributo Connected Body desse componente. Pronto. Podes testar e agora o Grab só funciona no Trinco. 

Mas você pode perceber que a abertura e mantê-la aberto está muito mais sensível (e mais realista). Logo, o ângulo de abertura da porta não necessariamente vai chegar a 120º. Por isso, vamos corrigir o código de EventosPorta para verificar ângulos menores ou maiores que 40º. Isso já dá uma abertura suficiente para passarmos. 

`void Update() { float angle = hinge.angle; // abriu if (!isOpen && angle` **`<= -40`** `) { isOpen = true; teleporte.enabled = true; } else { // Porta fechou if (isOpen && angle` **`> -40`** `) { isOpen = false; teleporte.enabled = false; } } }` 

## **2.4. Porta de Correr** 

Vamos adicionar uma porta de correr que liga a sala dos botões com o Mundo exterior. 

Inicie importando o pacote https://assetstore.unity.com/packages/3d/props/interior/barn-door-asset-pack-272809. Window > Rendering > Rendering Pipeline Converter > Selecione todas as caixas e clique em [Initialize and Convert]. 

Você precisa organizar seguindo o mesmo princípio que foi criado no último tutorial para as paredes e a abertura para a porta. Comece colocando uma porta na parede externa para ter noção do espaço para criar a passagem. Na pasta Assets >  BarnDoorAssetPack > Prefabs escolha uma porta e coloque rente à parede externa da segunda sala dando acesso ao exterior. Altere a escala dela para 1.5. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

Com base nisso redimensione e/ou reposicione uma das paredes e coloque as outras uma na parte de cima da porta e a outra no outro lado da abertura. 

Para facilitar o desenvolvimento dessa porta, coloque o XR Origin na segunda sala olhando para a porta. Arraste a guia Game do lado da Scene para conferir e ajustar a abertura da porta. Não esqueça de atribuir às paredes o layer Obstaculos. A porta faremos essa configuração mais tarde. 

Expanda na guia hierarquia o elemento dessa porta. O primeiro elemento filho se chama Hanger. Dentro dele tem outro Hanger e um elemento Rail. Quando eu me referir ao elemento Hanger, considere sempre esse segundo. 

Adicione um BoxCollider nesse Hanger e altere a altura dele conforme a figura abaixo. Lembre-se que para alterar os limites de um BoxCollider precisa clicar no botão Edit Collider no componente BoxCollider. Ele tem que ficar parecido com a imagem abaixo. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202603 

Como não existe um elemento para a maçaneta, dessa vez vamos fazer para que o Grab seja completamente na porta. Adicione o componente Rigidbody>Collision Detection mude para Continuous Dynamic. Vá ao elemento filho BarnDoor_NN que está dentro de Door. Adicione um componente BoxCollider. Ele criará o Collider já ajustado. Adicione um componente XR Grab Interactable e o atributo Movement Type mude para Velocity Tracking. Em Rigidbody>Collision Detection mude para Continuous Dynamic. Adicione o componente Fixed Joint e arraste Hanger para o atributo Connected Body desse componente. Execute e vamos ao primeiro teste da porta usando a tecla G. Você vai conseguir mover a porta livremente. Vamos novamente usar um tipo de Joint para limitar esse movimento com Grab. 

Selecione Hanger e adicione o componente Configurable Joint.  Olhando para o Gizmo no canto superior direito, eu consigo concluir que a porta vai deslizar somente pelo eixo X. Logo preciso limitar a porta para << correr somente nesse eixo. Nos atributos Y Motion, Z Motion, e aqueles Angular [X, Y e Z] Motion atribua Locked. Já no atributo X Motion atribua Limited. No campo Linear Limit>Limit deixe 1.2. Executando o jogo você observa que funciona o deslocamento somente no eixo que especificamos, mas temos duas coisas para resolver: a porta está bem bamba e limitar para correr para abrir somente à direita. 

O primeiro problema é resolvido atribuindo Obstaculos no Layer do Hanger (somente nele, responda com Não para os filhos), para que ele considere todas as configurações de tratamento da física como já havíamos configurado para porta. Execute e perceba que a porta está mais estável. 

Para limitar a porta para não correr à esquerda, teremos que adicionar um collider para definir essa fronteira. Primeiro, crie um elemento vazio, dentro do primeiro Hanger e renomeie como Batente. Veja a ilustração ao lado. Depois, dentro desse elemento, adicione um BoxCollider e edite seus limites na borda da porta, pelo lado de fora, conforme a imagem a seguir. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

Pronto. Teste agora e verá que o comportamento da porta está ok. Porém, o teleporte para fora está desabilitado. Como na primeira porta, vamos criar um script para habilitar essa porta. Adicione um novo componente em Hanger chamando ele EventosPortaCorrer e crie o script. Mais tarde, por uma questão de organização, arraste esse script para pastas Scripts. Abaixo está o código do script. 

`using UnityEngine; using UnityEngine.Events; using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation; public class EventosPortaCorrer : MonoBehaviour { private bool isOpen = false; private ConfigurableJoint joint; public TeleportationArea teleporte; void Start() { joint = GetComponent<ConfigurableJoint>(); } float GetJointLinearX() { // Calcula posição do anchor no mundo Vector3 worldAnchor = joint.transform.TransformPoint(joint.anchor); Vector3 connectedAnchor = joint.connectedAnchor; // Delta entre anchors Vector3 delta = worldAnchor - connectedAnchor; // Eixo X do joint no espaço global Vector3 axisX = joint.transform.TransformDirection(Vector3.right); // Projeção do deslocamento no eixo X float displacementX = Vector3.Dot(delta, axisX); return displacementX; } void Update() { float abertura = Mathf.Abs(GetJointLinearX()); // abriu if (!isOpen && abertura >= 0.6) { isOpen = true; teleporte.enabled = true; } else { // Porta fechou if (isOpen && abertura < 0.6) { isOpen = false; teleporte.enabled = false; } } } }` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

O valor 0.6 me parece um tamanho suficiente. Mas você pode ajustar para exigir uma abertura maior. Salve. Retorne ao editor e espere compilar. Veja se o Hanger tem como componente esse Script, senão adicione. Arraste a área de teleporte de fora da casa para o atributo Teleporte do componente EventosPortaCorrer. Garanta que o script é um componente de Hanger. 

Para finalizar, vamos acertar para que, quando o usuário estiver na primeira sala, e apontar para essa porta, não funcione o arrastar. Selecione BarnDoor_nn e desabilite XRGrabInteractable, desmarcando a caixa de verificação ao lado dele. 

Acesse o código EventosPorta e faça as seguintes modificações. 

`...` **`using UnityEngine.XR.Interaction.Toolkit.Interactables;`** `public class EventosPorta : MonoBehaviour { ...` **`public XRGrabInteractable grabPorta;`** `... void Update() { ... teleporte.enabled = true;` **`grabPorta.enabled = true;`** `} else { ... teleporte.enabled = false;` **`grabPorta.enabled = false;`** `} } ...` 

Coloque XROrigin na primeira sala. No componente EventosPorta de Espelho, para o atributo Grab Porta, arraste o elemento BarnDoor_nn. Pronto. Pode testar o jogo. 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S. Para compilar as alterações no código, também basta salvar o script. 

## **3. Saindo do laboratório** 

Agora vamos aprender a adicionar um texto que vai ser mostrado ao jogador (chamamos normalmente de UI) quando ele sair do laboratório. 

Em Chao3, adicione um componente script ShowMessageNaArea. Não esqueça de arrastar para a pasta Scripts. 

`using UnityEngine; using UnityEngine.XR.Interaction.Toolkit.Locomotion; using UnityEngine.XR.Interaction.Toolkit.Interactors; public class ShowMessageNaArea : MonoBehaviour { public Transform playerCamera; public GameObject messageObject; public LocomotionMediator locomotionSystem; public XRRayInteractor leftRay; public XRRayInteractor rightRay; public float maxDistance = 3f; private bool triggered = false;` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

`void Update() { if (triggered) return; Ray ray = new Ray(playerCamera.position, Vector3.down); RaycastHit hit; if (Physics.Raycast(ray, out hit, maxDistance)) { if (hit.collider.gameObject == gameObject) { messageObject.SetActive(true); Time.timeScale = 0; // pausar o jogo locomotionSystem.enabled = false; // nao conseguir mais andar leftRay.enabled = false; // desativa linha de teleporte do controle esquerdo rightRay.enabled = false; // desativa linha de teleporte do controle direito triggered = true; } } } }` 

Salve e retorne ao editor. Na hierarquia, com o botão direito escolha UI > Text – TextMeshPro. Com isso será criado um elemento Canvas e dentro dele Text (TMP). Selecione esse elemento, e na guia Inspector, no campo Text Input digite Você venceu !. Aproveite para redimensionar a caixa para que o texto caiba em uma única linha. Selecione Canvas, guia Inspector, clique na caixa de verificação ao lado do nome do elemento para desativá-lo. 

Abra a hierarquia do XR Origin pois precisaremos ter visível Camera Offset e Locomotion. Inclusive abra esse Camera Offset > Left Controller e Right Controller para que fique visível Teleport Interactor de ambos. 

Clique no elemento Chao3, componente ShowMessageNaArea e arraste Camera Offset para Player Camera, Canvas para Message Object, Teleport Interactor do Left Controller para Left Ray e do Right Controller para Right Ray. 

Execute e teste pisar no Chao3. É claro que você vai incrementar essa mensagem, colocando um pano de fundo para bloquear a visão do jogador e um botão para sair do jogo. Mas está feita a funcionalidade básica. 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S. Para compilar as alterações no código, também basta salvar o script. 

## **4. Compilação e Distribuição** 

## **4.1. Modo desenvolvedor** 

Aqui estou deixando anotada essa seção caso você venha a ter um dispositivo. Todos os dispositivos da Udesc já passaram por essa configuração. Antes de você conseguir subir aplicações que você vai construir para os Oculus Quest, é preciso configurá-los, para estarem no modo desenvolvedor. Siga os passos abaixo para habilitar o seu Oculus nesse modo. 

1. Ligar headset 

2. Abrir aplicativo Meta Horizon do celular 

3. Clicar no menu dispositivos 

4. Parear um Headset 

5. Escolha Quest 2 ou 3 (conforme a disponibilidade) 

Após parear, clique no menu relacionado a esse headset. 

1. Configurações do headset 

2. Modo de desenvolvedor 

3. Habilite esse modo. 

**4.2. Compilar** 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

No Unity, File > Build Profiles, clique em Android e Switch Platform. Em Scene List verifique se é MainScene que está selecionada (e basta só ela). Se tiver outras cenas, podes desmarcar todas, senão vai gastar seu tempo à toa quando compilar. Se precisar configurar essa cenas, basta clicar em [Open Scene List]. Clique no botão [Build] e selecione uma pasta de destino. Na primeira compilação poderá demorar significativamente. Mas, da segunda em diante já será mais rápido. Esse é um processo que você faz em casa, e depois traz o **apk** resultante para a Udesc para você experimentar o próximo passo na Universidade. 

## **4.3. Disponibilizar aplicativo** 

Agora eu vou explicar o que precisas fazer para enviar o **apk** para o Oculus. A primeira coisa a ser feita é ligar o Oculus. Após ligá-lo, conecte um cabo USB-C no computador e vai aparecer no Oculus uma mensagem como a da figura abaixo, para habilitar a transferência de arquivos. O mesmo acontece quando você fez o mesmo procedimento no seu celular. 

Vamos utilizar uma ferramenta do Android executável do Prompt. Para isso, precisamos saber onde estão instaladas essas ferramentas. No Unity Edit>Preferences>External Tools e no campo Android SDK Tools Installed With Unity está indicada a pasta. Daqui por diante vou chamá-la de ANDROID_SDK_HOME. Vou supor que você tem na unidade F: o arquivo FIRSTVR.apk. 

Vá ao prompt e digite: 

<ANDROID_SDK_HOME>\platform-tools>adb device 

Deve listar um dispositivo com um id parecido com 1WMHH841LF1325. Isso significa que está tudo OK para transferir o arquivo. De novo no prompt: 

<ANDROID_SDK_HOME>\platform-tools>adb install f:FIRSTVR.apk 

No Oculus, procurar por aplicativos e fontes desconhecidas. Você verá uma opção conforme definiu em Edit>Project Settings>Player como explicado no primeiro tutorial. 

Caso, você tenha um Oculus em casa, siga os passos para conectá-lo e habilitar o USB Debugging. Na janela Build Profiles, no campo Run Device, deve aparecer o seu dispositivo da lista (algo parecido com 1WMHH841LF1325). Então você pode clicar no botão [Build and Run] para compilar, subir o aplicativo para o dispositivo e já executar. 

Se após executar o comando **adb devices** aparecer uma mensagem _please add debug authorization and reconnect_ , temos que seguir alguns passos para restaurar essa conexão. Desconecte o Oculus do cabo USB. No aplicativo do celular, com ele pareado, vá para as opções do Modo Desenvolvedor e clique em "Revoke USB debugging authorization". No computador execute os seguintes comandos no prompt: **adb kill-server** e depois **adb start-server** . Reabilite o Modo Desenvolvedor no celular. Reconecte o dispositivo no cabo USB, e siga todos os passos acima desde a confirmação na janela do USB Debugging. 

## **4.4. Primeira execução da aplicação** 

É sacanagem essa ser a última sessão. Mas é interessante você perceber que está tudo certo nos seus testes no PC mas quando vai para o Oculus uma coisa tão primária não funcionou de acordo. O seu personagem fica numa posição muito rente ao chão. Para resolver isso, abra XR Origin e modifique o Tracking Origin Mode para Device. E torcer para 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202603 

você ter visto isso antes das próximas aulas que vou levar os dispositivos para você sentir o comportamento da tua aplicação. 

