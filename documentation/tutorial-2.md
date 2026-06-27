DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202604 

O estilo de jogo criado no tutorial é um Escape Room baseado num laboratório de química. 

Nesse tutorial vamos desenvolver parte do cenário e implementar as primeiras interações. 

Este documento tem uma duração estimada de 4 horas/aula, ou seja 3 horas e 15 minutos, aproximadamente. Execute o tutorial com calma e tranquilidade. Tenha certeza de que está cumprindo com todos os passos. 

## **0. Chão** 

Primeiro, vamos aumentar o tamanho do chão. Clique nesse objeto em [Hierarchy]. Na guia [Inspector], nos campos X e Z de Scale, modifique de 1 para 2. 

## **1. Cenário** 

Vamos acessar assets prontos para criarmos nosso cenário. Nessa versão do tutorial temos disponível assets para criar um laboratório de química. Acesse https://assetstore.unity.com/packages/3d/environments/chemistry-lab-itemspack-220212 e clique em [Add to my assets] e [Accept]. Em seguida vá para Window > Package Manager e My Assets. 

Talvez seja necessário clicar em [Refresh] localizado na parte inferior de [My Assets]. Procure por [Chemistry Lab Items Pack], clique em [Download], e depois de ter baixado em [Import X.X.X to Project] e [Import]. Fechando essa janela você vai reparar que na pasta [Assets] foi criada uma pasta [3D Laboratory etc etc]. 

Adicione esse pacote de ferramentas para usarmos mais tarde para contornar alguns objetos: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488. Siga os mesmos procedimentos do parágrafo acima. 

Quando os assets estão rosa significa que o material do Shader (responsável por pintar os pixeis) está quebrado. Para resolver isso Window > Rendering > Render Pipeline Converter e na janela que abre selecione as quatro (ou cinco) opções e clique em [Initialize and Convert]. 

Acesse Assets > 3D Laboratory etc etc. > Prefabs. Se eles estiverem em cor de rosa, execute o procedimento acima. Depois desse processo, ainda vão manter 3 assets com essa cor. É que seus materiais não foram enviados nesse pacote 

. 

## **1.1.Quatro paredes** 

Vamos iniciar com um cenário simples para você ir entendendo o desenvolvimento de um aplicação com RV. A ideia nesse momento é criar uma sala para você experimentar os três tipos de interação. 

Para as paredes eu adicionei 3D Laboratory etc etc. >PreFabs> wall01.  Deixei a primeira com position x=7, y=0 e z=0. Depois na própria [Hierarchy] vou copiando e colando, e alterando o x sempre subtraindo 4.25 que é o tamanho da parede. Logo a segunda parede fica x=2.75 e a terceira com x=-1,5. E fui seguindo essa lógica (somando ou subtraindo 4.25 do x ou do z dependendo da situação) adicionando as paredes para os quatro lados. As paredes laterais use Rotation Y=90. Para fazer os acabamentos nas junções das quatro paredes adicionei o prefab pillar. XROrig eu deixei no z=5 (ou  z=-5, dependendo da direção do quarto) para centralizar o personagem. O primeiro resultado ficou conforme a figura a seguir. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202604 

## **Movimento contínuo** 

Em outra aula vamos entrar em mais detalhes sobre esse assunto, para discutir até questões de conforto. Porém, para testes, nessa aula vamos usar esse tipo de movimento (WASD). Execute o jogo e experimente ultrapassar as paredes. O comportamento padrão do XR não bloqueia o deslocamento do jogador, então você precisa tratar isso explicitamente. Existem duas formas que os jogos resolvem esse problema quando o jogador tentar atravessar uma parede: 

1. Efeito Fade to Black: Escurece a tela e ele é teleportado para trás; 

2. Efeito Push Back: Colisão com a cabeça do jogador e empurra ele para trás. 

Vamos usar a segunda alternativa que é considerada a mais professional. 

Vá em Assets, crie uma pasta Scripts e dentro dessa pasta crie um script HeadCollisionDetector. Copie e cole código abaixo, que serve para detectar Layers à frente. As linhas Serialized Field indicam atributos que ficarão disponíveis para alterar no Editor. 

`using System.Collections; using System.Collections.Generic; using UnityEngine; public class HeadCollisionDetector : MonoBehaviour { [SerializeField, Range(0, 0.5f)] private float _detectionDelay = 0.05f; [SerializeField] private float _detectionDistance = 0.2f; [SerializeField] private LayerMask _detectionLayers; public List<RaycastHit> DetectedColliderHits { get; private set; } private float _currentTime = 0; private List<RaycastHit> PreformDetection (Vector3 position, float distance, LayerMask mask) { List<RaycastHit> detectedHits = new(); List<Vector3> directions = new() { transform.forward, transform.right, -transform.right }; RaycastHit hit; foreach (var dir in directions) {` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

`if (Physics.Raycast(position, dir, out hit, distance, mask)) { detectedHits.Add(hit); } } return detectedHits; } private void Start() { DetectedColliderHits = PreformDetection(transform.position, _detectionDistance, _detectionLayers); } void Update() { _currentTime += Time.deltaTime; if (_currentTime > _detectionDelay) { _currentTime = 0; DetectedColliderHits = PreformDetection(transform.position, _detectionDistance, _detectionLayers); } } }` 

Salve para compilar. Na guia [Hierarchy] em, XR Origin>Camera Offset>Main Camera e crie um elemento vazio (Create Empty). Renomeie para Detector. Selecione esse elemento, na guia [Inspector] clique em [Add Component] e selecione HeadCollisionDetector. Selecione uma parede, e no campo Layer (fica bem em cima na guia Inspector) clique em [Add Layer] e digite Obstaculos em User Layer 3 (ou qualquer outra Layer que você tiver disponível). Agora selecione todas as paredes e os pilares (usando a tecla Shift) e atribua Obstaculos em Layer. No futuro, não esqueça de atribuir esse Layer aos seus componentes. No componente Detector, selecione Obstaculos em Detection Layers. 

Crie um script HeadCollisionHandler. Copie e cole o código abaixo. Esse código será integrado ao código anterior, para fazer acontecer o processo de Push Back. 

`using System.Collections; using System.Collections.Generic; using UnityEngine; public class HeadCollisionHandler : MonoBehaviour { [SerializeField] private HeadCollisionDetector _detector; [SerializeField] private CharacterController _characterController; [SerializeField] public float pushBackStrength = 1.0f; private Vector3 CalculatePushBackDirection(List<RaycastHit> colliderHits) { Vector3 combinedNormal = Vector3.zero; foreach (RaycastHit hitPoint in colliderHits) { combinedNormal += new Vector3(hitPoint.normal.x, 0, hitPoint.normal.z); ; } return combinedNormal; } private void Update() { if (_detector.DetectedColliderHits.Count <= 0) { return; }` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202604 

`Vector3 pushBackDirection = CalculatePushBackDirection(_detector.DetectedColliderHits); Debug.DrawRay(transform.position, pushBackDirection.normalized, Color.magenta);` 

`_characterController .Move(pushBackDirection.normalized * pushBackStrength * Time.deltaTime); } }` 

Salve para compilar. [Add Component] em MainCamera, adicionando esse script. Para o atributo Detector desse script arraste o componente Detector, e para o atributo Character Controller arraste XR Origin. 

Execute o jogo e tente atravessar as paredes. Existe um problema quando você andar para trás. **Vamos ignorar pois o movimento contínuo será abolido (ou limitado) na próxima aula.** 

Atenção: não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S, pois não há salvamento automático. Para compilar as alterações no código, também basta salvar o script. 

## **1.2.Tubos de Ensaio** 

Primeiro, vamos criar uma bancada de trabalho. A partir de Prefab, adicione três Shelf showcase (são as bases) e dois shelf. Posicione eles para ficarem mais ou menos assim. 

Na pasta Models, procure por Test_tube_rack. Adicione dois desses sobre a bancada. Crie um material (na pasta Materiais) chamado MaterialRack. Selecione esse material, e na guia [Inspector] em Base Map selecione um cor que você gostaria que esses racks tivessem. Depois nessa mesma guia, eu atribui 1 para Metallic Map para deixar com aparência de metal para esses racks. Arraste esse material nesses racks que acabou de adicionar. 

Vamos adicionar alguns tubos de ensaio nesse rack. Começaremos adicionando alguns tubos vazios para enfeitar. Na pasta Models, procure por Glass_Lab_test_tube. Adicione alguns no rack. Divirta-se com o posicionamento deles. O mais produtivo é adicionar e posicionar um primeiro, e depois faça cópias dele só para alterar um dos eixos. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

Agora vamos adicionar alguns tubos com líquidos. Procure por “Glass_Lab_test_tube with liquid” na pasta Prefabs e adicione dois tubos em um rack e um no outro. Deixe os dois do mesmo rack bem afastados pois iremos permitir que eles possam ser segurados. Depois de adicionar os tubos, podemos personalizar a cor e a quantidade de cada líquido. Em [Hierarchy], abra um dos objetos “Glass_Lab_test_tube with liquid” desse tubo, e dentro dele existe um objeto “Glass_Lab_test_tube6”. Abra a pasta 3D Laboratory > Materials e observe que existem quatro materiais chamados “Liquid” (1, 2, 3 e 4). Arraste um dos materiais para esse objeto “Glass_Lab_test_tube6” para ver sua cor modificar. 

Para criar uma cor, basta copiar um dos materiais “Liquid n” e colar na mesma pasta. Depois, vá na guia [Inspector] desse novo material e altere sua cor em Base Map e Emission Map. 

Para modificar a quantidade de líquido dentro do tubo, selecione o objeto “Glass_Lab_test_tube6” e na guia [Inspector] altere o Y do scale. 

Para finalizar vamos adicionar um copo de béquer onde mais tarde poderemos colocar o conteúdo dos tubos de ensaio. Em Prefabs, selecione “Beaker water” (aquele que não está rosado) e arraste para cima da mesa. Em [Hierarchy] abra o objeto, selecione “Beaker liquid” e altere o atributo Y do scale para 0. E por fim, desabilite esse objeto para desaparecer de vez. 

Vamos abrir um grande parêntesis na criação do cenário para explicar uma das features essenciais de aplicações com RV. 

## **2. Interação com objetos** 

Vamos começar apontando três conceitos essenciais para essa parte: 

- **Interactors:** em [Hierarchy]>[XR Origin]>[Camera offset]>[Left Controller] ou [Right Controller] e você notará três componentes: Poke Interactor, Near-Far Interactor e Teleport Interactor. Representam o disparo de eventos a partir dos controles. Esses componentes possuem as configurações de como se devem comportar as ações dos controles; 

- **Interactables:** serão adicionados componentes naqueles objetos que podem ser pegos ou interagidos; 

- **Interaction Manager:** em [Hierarchy]>[XR Interaction Manager] é o componente que coordena a funcionalidade de ambos. 

## **2.1. Near-Far Interactor** 

Vamos iniciar com o mais essencial da interação que é segurar os objetos. Primeiro vamos ver o funcionamento de segurar um tubo de ensaio livremente e depois vamos fazer adaptações para reproduzir o funcionamento de tirar o tubo de ensaio do rack e virar ele quando encostado no copo de béquer. 

Clique em dos tubos de ensaio com líquido (Glass_Lab_test_tube with liquid), e na guia Inspector clique [Add Component]. Localize e adicione XR Grab Interactable: esse adiciona a capacidade do asset de ser manipulado pelos controles. Adicionando esse componente é automaticamente adicionado Rigidbody. De Rigibody, o atributo Interpolate mude para Interpolate, e Collision Detection para Continuous Dynamic. Isso evitará atravessar objetos quando solta o tubo de ensaio. Em XR Grab Interactable, mude o atributo Movement Type para Velocity Track e desabilite Thrown on Detach. Isso vai permitir uma física mais realista e que o tubo não seja arremessado. Experimente executar o jogo, posicione o seu personagem em frente ao rack (com WASD), pressione [Tab] para trocar a 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202604 

movimentação do corpo pelos controles e [ ] ] para habilitar somente o controle direito. Quando a bolinha na ponta do controle estiver laranja, pressione [G] e mova o tubo. 

Você pode observar que o controle se baseia em movimentar a ponta inferior do tubo. Vamos alterar para que você segure a ponta superior. 

Na [Hierarchy], abra esse tubo de ensaio para que possamos adicionar outro elemento dentro dele. 

Botão direito e [Create empty]. Renomeie para “AttachPoint”. No meu caso, eu tive que colocar o Y do Position em 0.23 para deixar a ponta para fora do rack. Selecione o tubo de ensaio, e procure o atributo Attach Transform em XR Grab Interactable. Arraste o objeto AttachPoint para esse atributo. Ao executar o jogo, e fazer as mesmas interações de agarrar, perceberá que ele segura na ponta do tubo. 

Mas existe uma situação: você consegue remover de qualquer jeito esse tubo do rack. Vamos alterar para que ele saia no eixo Y inteiramente antes de mover livremente. Na pasta Assets > Scripts use o botão direito e selecione Create > Scripting > Empty C# Script. Renomeie para TestTubeLock e cole o código abaixo. 

`using UnityEngine; using UnityEngine.XR.Interaction.Toolkit; public class TestTubeLock : MonoBehaviour { [SerializeField] private Transform rackTopLimit; // ponto acima do rack private XRGrabInteractable grab; private Rigidbody rb; private bool isLockedInRack = true; void Start() { grab = GetComponent<XRGrabInteractable>(); rb = GetComponent<Rigidbody>(); LockInRack(); } void Update() { if (isLockedInRack) { CheckIfReleased(); } } void LockInRack() { rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ; } void UnlockFromRack() { rb.constraints = RigidbodyConstraints.None; isLockedInRack = false; }` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

`void CheckIfReleased() { if (transform.position.y > rackTopLimit.position.y) { UnlockFromRack(); } } }` 

Salve e [Add Component] no tubo de ensaio (Glass_Lab_test_tube with liquid) adicionando este script. Dentro do rack (Test_tube_rack) onde está esse tubo crie um Empty Object chamado TopLimit. Atribua o Y de acordo com a posição na altura onde o tubo já saiu do buraco. No meu caso (e provavelmente no seu também) será 0.227. Arraste esse objeto para o atributo RackTopLimit do componente TestTubeLock do tubo de ensaio. Execute o jogo, e vá bem próximo do rack. Experimente primeiro levantar o tubo sem tirar do rack e solte ele. Depois experimente remover o tubo fora do rack. Vais perceber que ele se move livremente. 

Para finalizar essa parte da relação entre tubo e rack, vamos agora permitir que o jogador coloque de volta o tubo no rack. Para isso crie mais um objeto empty dentro do mesmo Test_tube_rack. Chame de Socket (depois quando você tiver que criar dois desses no outro rack com dois tubos, chame um de Socket1 e outro de Socket2). Adicione um Sphere Collider com Radius de 0.015 e marque a caixa de verificação Is Trigger. Centralize o Socket (e automaticamente o Collider vai acompanhar, mas tome cuidado para não deslocar somente o Collider) no centro do furo que está o tubo de ensaio. 

Abaixo está ilustrado todo o código do TestTubeLock com as linhas adicionais em negrito e vermelho. 

`using UnityEngine; using UnityEngine.XR.Interaction.Toolkit; public class TestTubeLock : MonoBehaviour { [SerializeField] private Transform rackTopLimit; // ponto acima do rack` **`[SerializeField] private Transform socketPoint; // ponto no meio do furo do rack`** `private XRGrabInteractable grab; private Rigidbody rb; private bool isLockedInRack = true; void Start() { grab = GetComponent<XRGrabInteractable>(); rb = GetComponent<Rigidbody>(); LockInRack(); } void Update() { if (isLockedInRack) { CheckIfReleased();` 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

`} } void LockInRack() { rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;` **`isLockedInRack = true;`** `} void UnlockFromRack() { rb.constraints = RigidbodyConstraints.None; isLockedInRack = false; } void CheckIfReleased() { if (transform.position.y > rackTopLimit.position.y) { UnlockFromRack(); } }` **`private void OnTriggerEnter(Collider other) { if (other.CompareTag("TubeSocket") && !isLockedInRack) { SnapToSocket(); } } void SnapToSocket() { transform.position = socketPoint.position; transform.rotation = socketPoint.rotation; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; LockInRack(); }`** `}` 

Crie uma nova tag chamada TubeSocket. Para criar a tag é o mesmo princípio dos Layers. Vá em qualquer objeto, e veja que existe um campo Tag do lado do campo Layer. Clique no botão para abrir a lista e depois em [Add Tag...]. 

Na nova janela, em Tags clique no botão [+] e no campo que se abre digite TubeSocket e clique em [Save]. Selecione Socket na [Hierarchy] e no campo Tag selecione TubeSocket. 

Selecione o tubo de ensaio, e arraste Socket para o atributo Socket Point do componente TestTubeLock. Execute o jogo e verá que além de controlar o Grab para remover o tubo do rack, quando tiver tirado todo ele, e se aproximar no rack, o tubo entrará automaticamente no rack. 

Agora aplique tudo que viu nessa seção nos outros dois tubos de ensaio com líquido. Também podes fazer copiar e colar, não esquecendo também do AttachPoint e dos dois Sockets. Nesse caso, esses três objetos precisam ser reposicionados (tal como os tubos de ensaio) e usar eles como referência no script TestTubeLock. 

Agora vamos finalizar as interações com o tubo de ensaio. Vamos criar um sistema que incline o tubo de ensaio (simulando derramar o líquido dentro dele) somente quando estiver próximo do copo de béquer. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

No copo de béquer (Beaker water) adicione um objeto vazio ([Create Empty]). Renomeie para ZonaDerramar. No Y de Position altere para 0.11. Adicione um componente Sphere Collider com Radius de 0.046. Marque a opção Is Trigger. Essa será a zona que o tubo de ensaio deve entrar para ficar no modo derramamento. Cria uma Tag ZonaDerramar. Atribua essa Tag ao objeto ZonaDerramar. Crie um script na pasta Scripts com o seguinte código. 

`using UnityEngine; public class DerramarLiquido : MonoBehaviour { [SerializeField] private float anguloMin = 100f; // Ângulo mínimo para considerar "virado" private bool inZonaDerramar = false; private bool estahDerramando = false; void Update() { if (!inZonaDerramar || estahDerramando) return; float angulo = Vector3.Angle(transform.up, Vector3.up); if (angulo > anguloMin) { Pour(); } } void Pour() { estahDerramando = true; print("Ingrediente derramado!"); } private void OnTriggerEnter(Collider other) { if (other.CompareTag("ZonaDerramar")) { inZonaDerramar = true; print("In ZonaDerramar!"); } } private void OnTriggerExit(Collider other) { if (other.CompareTag("ZonaDerramar")) { inZonaDerramar = false; print("Out ZonaDerramar!"); } } }` 

Adicione esse script aos três Glass_Lab_test_tube with liquid. Execute o jogo e faça umas manobras para colocar o tubo. Talvez leve a uma frustração pela dificuldade de usar o simulador. Mas se você seguiu esse tutorial para que tudo esteja funcionando corretamente, quando compilar e testar no Oculus, perceberá que o funcionamento de derramar o líquido do tubo será muito natural. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202604 

## **2.1.1. Adicionar um contorno nos tubos de ensaio** 

O procedimento que será explicado aqui precisa ser aplicado nos três tubos de ensaio. Vou explicar fazendo em um deles, e depois você replica nos outros dois. 

Na guia Hierarchy, selecione o tubo de ensaio (Glass_Lab_test_tube with liquid), e adicione um componente (botão Add Component na guia Inspector) Outline (o primeiro com ícone de #). Para configurar as cores na guia Inspector, no componente recém adicionado, clique em Outline Color. Na janela Color, clique no círculo externo para alcançar a cor de sua preferência, e no retângulo interno selecione a tonalidade dessa cor. Para o Outline Width coloque como zero. 

Em DerramarLiquido, você vai precisar adicionar duas linhas, destacadas em vermelho. 

`private void OnTriggerEnter(Collider other) { if (other.CompareTag("ZonaDerramar")) {` **`gameObject.GetComponent<Outline>().OutlineWidth = 5f;`** `inZonaDerramar = true; print("In ZonaDerramar!"); } } private void OnTriggerExit(Collider other) { if (other.CompareTag("ZonaDerramar")) {` **`gameObject.GetComponent<Outline>().OutlineWidth = 0f;`** `inZonaDerramar = false; print("Out ZonaDerramar!"); } }` 

Execute e experimente colocar o tubo de ensaio na boca do copo de béquer, e verá o tubo ficar com um contorno da cor de sua preferência. 

