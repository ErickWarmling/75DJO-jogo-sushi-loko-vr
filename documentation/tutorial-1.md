DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202602 

Nesse projeto inicial vamos aprender a fazer um jogo de exploração. Você conseguirá usar caminhar, usar equipamentos, abrir portas, subir escadas, usar uma arma e enfrentará inimigos que podem atacar você quando estiverem perto. A ideia desse tutorial é dar uma introdução ao desenvolvimento de aplicações de RV com Unity. Usaremos os dispositivos da Meta (Oculus Quest 2 e 3) e a biblioteca OpenXR Foundation. Existe uma lista de requisitos para uma melhor experiência com a visualização do aplicativo de RV no computador. Não desanime se não atende os requisitos. Mais adiante vou apresentar que esse tutorial foi feito num equipamento que não atende a um desses requisitos. Por curiosidade, acesse https://www.meta.com/help/quest/140991407990979/. 

Esse tutorial está dividido em quatro arquivos. Esse tutorial está em construção. Se você tiver alguma sugestão de melhoria ou encontrar algum erro, comunique-me por e-mail > adilson.vahldick@udesc.br. 

Este documento tem uma duração estimada de 2 horas/aula, ou seja 1 hora e 40 minutos, aproximadamente. Execute o tutorial com calma e tranquilidade. Tenha certeza de que está cumprindo com todos os passos. 

## **1. Instalando Unity** 

Acesse http://unity.com e crie uma conta. Entre nessa conta, baixe e instale o Unity Hub. No Unity Hub, na guia Installs clique em Add e instale a versão **6000.0.68f1** usada nesse tutorial. Se não encontrar a versão, experimente acessar https://unity.com/pt/releases/editor/archive. 

Na nova janela, selecione Android Build Support (já vai marcar as duas abaixo dele) e Windows Build Support (IL2CPP). Clique em [Continue]. Na janela de consentimento marque a caixa de verificação [I have _etc_ ] e clique em [Install]. Agora é aguardar o _download_ e a instalação. Fique atento pois podem acontecer erros de validação na instalação. Basta clicar em repetir (simbolizado pelo botão com um círculo). 

## **2. Instalando ferramentas para RV** 

Agora vamos instalar a aplicação que será usada para conectar o computador aos Oculus da Meta (Meta Quest Link). Acesse o link https://www.meta.com/help/quest/1517439565442928/. Role a página até alcançar a imagem abaixo e faça download da aplicação. 

Em seguida é só executar a aplicação baixada, selecione onde será instalado e prosseguir com a instalação. No final, a aplicação será aberta, e precisaremos fazer algumas configurações. 

Vá no menu Configurações (ou Settings), na guia Geral (ou General) e habilite a opção Fontes Desconhecidas (ou Unknown Source). Depois vá na guia Beta habilite Recursos de tempo de execução para o desenvolvedor (ou Developer 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202602 

runtime features), Visão do ambiente pelo Meta Quest Link (ou Pass-through over Meta Quest Link) e Dados espaciais pelo Meta Quest Link (ou Spatial data over Meta Quest Link). 

Pode ser um tanto desanimador ter uma mensagem como a abaixo no seu Meta Quest Link. Isso acontece no meu computador; pois eu tenho NVIDIA GeForce GTX 1650, que não é suportado. Mas, pode se consolar, pois todas as aulas foram desenvolvidas nesse computador que não atende as especificações mínimas. 

No seu celular, é preciso instalar o aplicativo Meta Horizon (veja a imagem abaixo na Play Store). Esse aplicativo serve para gerir o ecossistema Meta Horizon. A partir dele você habilita a instalação dos aplicativos no seu Meta Quest, gerencia e configura os seus dispositivos. Para esse momento servirá apenas para lembrá-lo de criar uma conta na Meta. **Faça isso** . Mais tarde, você verá como habilitar o dispositivo para o modo desenvolvedor. 

## **3. Criar projeto** 

Agora vamos criar nosso primeiro projeto. Para isso, no Unity Hub, no meu lateral acesse Projects e na guia que se abre com essa opção, clique em [New Project]. Na seleção de _templates_ , selecione VR (se necessário faça o download), batize o nome do seu primeiro projeto ( _eu chamei de First VR_ ) em [Project name] e defina a pasta raiz que conterá seu projeto em [Location]. 

É interessante que você marque a opção [Use Unity Version Control]. Está fora do escopo desse tutorial explicações sobre sua utilização. Porém, é um sistema de versionamento de código fonte e arquivos binários mais especializado que o Github. De qualquer forma, sempre é possível usar o Github para realizar o versionamento. Mas aí você vai precisar ativar o LFS. Pesquise na internet sobre esse essas tecnologias. 

## **3.1. Interface do usuário** 

Abrirá uma janela parecida com a figura abaixo. Vamos passar as explicações de suas áreas. 

1-Hierarchy: para listar todos os objetos do projeto que podem ser exibidos na guia Scene; 

- 2-Scene: guia para edição do projeto; 

3-Game: o sistema em execução a partir da visualização da câmera principal (Main Camera); 

- 4-Inspector: apresenta os atributos do elemento selecionado na Hierarchy. Inclusive é possível alterar esses valores; 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202602 

5-Project: onde teremos acesso aos arquivos do projeto, por exemplo, a importação e acesso às texturas, aos arquivos de codificação (scripts), ...; 

- 6-Console: para visualizar e erros durante o desenvolvimento, ou da execução, alertas e logs de depuração. 

Não é necessário eu produzir mais uma documentação sobre as noções básicas da interface do Unity. Apesar da versão ser bem antiga, os links que eu selecionei apresentam o que é importante você saber sobre a interface. É importante você acompanhar essas páginas para seguir com naturalidade o tutorial. 

- https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/6 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/7 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/8 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/13 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/14 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/15 - https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/16 

- https://materialpublic.imd.ufrn.br/curso/disciplina/5/68/1/17 

Agora você sabe mudar a posição dos objetos, rotacioná-los e até mudar a escala deles. Apague o cubo que você criou nessas páginas, para prosseguirmos com o tutorial. 

## **4. Configurar projeto para executar aplicações de RV** 

Por padrão, os projetos estão configurados para serem compilados e executados no Windows. Entretanto, como desejamos que sejam executados nos óculos, precisaremos modificar a plataforma de destino para Android. 

File > Build Profiles e na nova janela está selecionado Windows como ativo. Selecione Android e clique em [Switch Platform]. Aguarde até o Unity ter compilado os scripts e importado os assets para desenvolver para dispositivos móveis. Depois que o editor liberou, pode fechar a janela de Build Profiles. 

Selecione Window > Package Manager. Vamos instalar um pacote que permitirá você simular os óculos na sua máquina, o que é primordial enquanto estamos desenvolvendo, pois mesmo tendo acesso a ele, não vale a pena 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202602 

compilar e fazer o upload para os óculos a todo instante para testar a aplicação. Selecione Unity Registry > XR Interaction Toolkit > Samples, e em **XR Interaction Simulator** , clique [Import]. Após a importação, feche a janela. 

Nesse tutorial não iremos explorar o recurso de manipulação com as mãos, mas se você quiser se aprofundar no desenvolvimento de RV, é recomendável explorar esse recurso. 

Selecione [Edit] > [Project settings] > [Player] e em Company Name, coloque seu nome completo, ou nick preferido. Sempre sem espaços. Para que a mudança seja efetivada, clique na guia [Android settings] (normalmente ela já está selecionada), role essa página até chegar em Identification e desmarque a caixa [Override Default Package Name], e depois volte a marcar essa caixa. Vais observar que o campo Package Name foi atualizado. 

Observe que apesar de estarmos desenvolvendo para testar nos Oculus Quest, vamos utilizar bibliotecas que permitirão você usar em outros dispositivos. OpenXR é um padrão aberto, inclusive, além de dispositivos de VR, permite a compatibilidade com dispositivos de RA. 

Na guia da esquerda, clique em [OpenXR], e em Enabled Interaction Profiles, clique no botão [+]. Repita o processo de clicar nesse botão, garantido as seguintes opções (caso alguma delas ainda não esteja selecionada): Oculus Touch Controller Profile, Meta Quest Touch Pro Controller Profile e Meta Quest Touch Plus Controller Profile. Repita o mesmo processo nas duas guias: Windows e Android. Ainda, na guia Android, em Open XR Feature Groups, marque Meta Quest Support. 

Ainda, na guia Android, selecione [Multi-pass] no campo Render Mode. Feche a janela. 

Não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S, pois não há salvamento automático. 

## **5. Criação da cena** 

Para criar uma cena vá nos menus [File] > [New Scene], selecione a opção Empty (existem outros tipos que podem te acelerar no processo de desenvolvimento, que você pode usar em um projeto futuro. Mas agora, vamos começar do zero para você entender a base) e clique em [Create]. Pressione [Ctrl] + [S] e entre nas pastas Assets > Scenes e nomeie como MainScene. 

Com botão direito em algum espaço em branco em [Hierarchy], (se não tiver uma esse objeto ainda) selecione [Light] > [Diretional Light]. Faça o mesmo com [XR] > [Interaction Manager]. Exclua a Main Camera caso ela exista. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202602 

Na guia [Projects] > [Assets] > [Samples] > [XR Interacion Toolkit] > [3.X.X] > [Starter Assets] > [Prefabs] e arraste para dentro de MainScene o asset [XR Origin (XR Rig)]. Clique nele e na guia [Inspector] garanta que os três campos de Position sejam iguais a 0. 

## **6. Adicionar o chão** 

Vamos criar um chão para o nosso cenário. Na guia [Hierarchy], clique com o botão direito numa área vazia, e selecione [3D Object] > [Plane]. Clique nesse elemento, e na guia [Inspector] mude o nome de Plane para Chao e garanta que todos os três campos de Position tenham o valor 0. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual Prof. Adilson Vahldick                                V202602 

Na guia [Project], deixe marcada a pasta [Assets], use o botão direito > [Create] > [Folder] > renomeie para Materiais. Entre na pasta Materiais e numa área vazia use o botão direito > [Create] > [Material] > renomeie para MaterialChao. Arraste o MaterialChao no objeto Chao. Verá que ele muda para branco. 

Vamos alterar algumas propriedades desse material. Com o MaterialChao selecionado na guia [Inspector], e clique na caixa branca do [Base Map] e abre uma nova janela Color. Nos campos R, G e B, troque o valor de 255 para 0, e a cor predominante será preta. Na guia [Inspector], no campo [Smoothness] mude de 0.5 para 0. 

## **7. Adicionar simulador** 

Por fim, vamos permitir simular o ambiente RV em nosso PC, e para isso vamos adicionar o simulador de controles. [Edit] > [Project settings] > [XR Interaction Toolkit] e deixe marcadas as opções [Use XR Interaction Simulator in scenes] e [Instantiate in Editor Only]. Feche a janela. Salve. 

Vamos verificar se o ambiente está pronto para ser explorado. Clique em Play e deve ser carregado na guia Game uma tela como a imagem abaixo na direita. 

Pressionando [Y], [Tab] e [X] abrem as janelas de ajuda das teclas. Pressionando [Tab] você alterna entre o corpo inteiro e a seleção de entrada. Clicando com o botão direito do mouse e mexendo o mouse você simula a movimentação dessas 4 opções (corpo inteiro, cada um dos controles e a cabeça). Quando você tiver usado o [Tab] vai movimentar ambos os controles. Com base em um teclado QWERTY, onde os caracteres [ e ] estão lado-a-lado (no meu teclado estão os acentos ao lado do caracter [), você alterna entre quais controles vai usar. Experimente clicar com o botão 1 para simular o pressionar do botão primário do controle direito. Ele vai simular um pulo. Pressionando a tecla H você configura para utilizar a cabeça. A tecla R recentraliza tudo. Para usar os botões e teclas no controle esquerdo, mantenha pressionada a tecla [Shift] e use as ações do controle direito. 

Quando o controle esquerdo está selecionado, as teclas IJKL são responsáveis pelo movimento. Quando o controle direito está selecionado, as teclas JKL são responsáveis pela rotação.  Brinque um pouco para se acostumar com os controles. 

DESO-Departamento de Engenharia de Software 75DJO-Desenvolvimento de Jogos OPT-Realidade Virtual 

Prof. Adilson Vahldick                                V202602 

Com o Game rodando, arraste a guia [Scene] para que ela fique disposta conforme a figura abaixo. Na guia [Hierarchy] clique em [XR Origin (XR Rig)]. Usando as teclas citadas no parágrafo anterior, você vai observar que o cone verde (representado pelo usuário) se moverá pelo cenário, e as linhas projetadas azuis serão rotacionadas. 

Observe que a rotação está girando de 45º em 45º (usando J ou L). Essa é a rotação mais aconselhada para reduzir o Cybersickness. Mas, se você quiser configurar para uma rotação contínua (experimente para ver como funciona), siga os seguintes passos: Pare a execução do jogo. Em [Hierarchy] selecione [XR Origin (XR Rig)] > [Camera offset] > [Right Controller], e na guia [Inspector] marque a opção [Smooth Turn Enabled]. 

Execute e teste as teclas J e L. 

Agora finalizamos a nossa primeira aula, com o setup do Unity, e a partir do próximo tutorial vamos aprender como adicionar interações. 

Não esqueça de salvar o seu projeto de vez em quando, com as teclas Ctrl + S, pois não há salvamento automático. 

