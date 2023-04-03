using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MapGenerator : MonoBehaviour
{
    // [SerializeField]
    private int worldWidth, worldDepth, worldHeight;
    // private Vector3[,,] vertices;
    // private int[] triangles;
    // private Mesh mesh;
    private bool initialised;

    /* ==================== ====================  ==================== ==================== */
    // noisemap data
    private float scale;
    private float persistance;
    private float lacunarity;
    private int seed;
    private float heightMultiplier;
    private int octaves;
    private Vector2 offset;

    /* ==================== ====================  ==================== ==================== */

    private float[,] noiseMap; // height map
    private int[,,] map; // map to store map information, whether point in above (0) or below (1) ground
    private int[,,] path; // stores path from start vec to end vec

    private int[,,] wallmap; // stores walls;

    /* ==================== ====================  ==================== ==================== */

    private GameObject cam; // 

    /* ==================== ====================  ==================== ==================== */

    private GameObject highlight, turretHighlight, walls, pathObj, enemyObj;

    [SerializeField]
    private Material __material, g1, g2, __highlight, __path, __wall;

    /* ==================== ==================== Enemy Data ==================== ==================== */

    private int wave;
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private EnemyScriptableObject edata;
    private Vector3 start, end;
    private float DistanceApart; // distance between start vec and end vec
    private List<Vector3> epath;
    private List<Vector3> eMovement;

    public int currentBuilding;
    private bool fastForward;
    [SerializeField]
    private List<GameObject> buildings;
    [SerializeField]
    private List<TurretScriptableObject> turretStats;

    public int money;
    private Dictionary<int, int> costs;

    public bool isBuilding;

    [SerializeField]
    private GameObject homebaseObj;
    private GameObject buildingObj;
    [SerializeField]
    private GameObject portal;

    [SerializeField] private List<GameObject> hotBarItems;

    /* ==================== ====================  ==================== ==================== */

    TextMeshProUGUI moneyText, fpsText, controlsText, waveText;
    GameObject startpos, endpos;

    /* ==================== ====================  ==================== ==================== */

    [SerializeField]
    private GameObject treeObj;

    /* ==================== ====================  ==================== ==================== */

    public void begin()
    {

        // GameObject.Find("Playing").SetActive(true);
        // GameObject.Find("MainMenu").SetActive(false);

        // this.buildings = new List<GameObject>();
        this.costs = new Dictionary<int, int>();
        this.money = 200;


        this.costs.Add(0, 10);
        this.costs.Add(1, 20);
        this.costs.Add(2, 50);

        this.wave = 1;
        this.currentBuilding = 0;

        this.worldWidth = 50;
        this.worldDepth = 50;
        this.worldHeight = 20;
        this.scale = 38.4f;
        this.persistance = 0.46f;
        this.lacunarity = 2.04f;
        this.seed = Random.Range(-1000, 1000);
        this.heightMultiplier = 2.4f;
        this.octaves = 3;
        this.offset = new Vector2(0, 0);
        this.DistanceApart = 40f;

        this.isBuilding = true;
        this.fastForward = false;

        //if (GameObject.Find("MapWidth").GetComponent<TMP_InputField>().text.Length != 0) { this.worldWidth = GameObject.Find("MapWidth").GetComponent<TMP_InputField>().text; }
        if (GameObject.Find("MapWidth").GetComponent<TMP_InputField>().text.Length != 0) { this.worldWidth = int.Parse(GameObject.Find("MapWidth").GetComponent<TMP_InputField>().text); }
        if (GameObject.Find("MapDepth").GetComponent<TMP_InputField>().text.Length != 0) { this.worldDepth = int.Parse(GameObject.Find("MapDepth").GetComponent<TMP_InputField>().text); }
        if (GameObject.Find("DistanceBetween").GetComponent<TMP_InputField>().text.Length != 0) { this.DistanceApart = int.Parse(GameObject.Find("DistanceBetween").GetComponent<TMP_InputField>().text); }
        if (GameObject.Find("Seed").GetComponent<TMP_InputField>().text.Length != 0) { this.seed = int.Parse(GameObject.Find("Seed").GetComponent<TMP_InputField>().text); }


        if (this.worldWidth <= 10 + this.DistanceApart) this.worldWidth = (int)(10 + this.DistanceApart + 1);
        if (this.worldDepth <= 10 + this.DistanceApart) this.worldDepth = (int)(10 + this.DistanceApart + 1);

        foreach (Transform canObj in GameObject.Find("Canvas").transform)
        {
            canObj.gameObject.SetActive(canObj.name == "Playing");
        }


        do
        {
            start = new Vector3(Random.Range(5, worldWidth - 5), 0, Random.Range(5, worldDepth - 5));
            end = new Vector3(Random.Range(5, worldWidth - 5), 0, Random.Range(5, worldDepth - 5));
        } while (Mathf.Abs(Vector3.Distance(start, end)) < DistanceApart);

        // this.mesh = new Mesh();
        // this.GetComponent<MeshFilter>().mesh = this.mesh;

        // this.vertices = new Vector3[(worldWidth + 1), (worldHeight + 1), (worldDepth + 1)];
        this.map = new int[(worldWidth), (worldHeight), (worldDepth)];
        this.path = new int[(worldWidth), (worldHeight), (worldDepth)];
        this.wallmap = new int[(worldWidth), (worldHeight), (worldDepth)];


        this.cam = GameObject.Find("Main Camera");

        this.highlight = new GameObject();
        this.highlight.transform.parent = this.transform;
        this.highlight.name = "highlight";
        int[,,] tmp = new int[1, 1, 1];
        tmp[0, 0, 0] = 1;
        CreateMesh(this.highlight, tmp, __highlight);
        this.highlight.GetComponent<MeshRenderer>().material = __highlight;

        GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereObjMesh = Instantiate(sphereObj.GetComponent<MeshFilter>().mesh);
        Destroy(sphereObj);

        this.turretHighlight = new GameObject();
        this.turretHighlight.transform.parent = this.transform;
        this.turretHighlight.name = "turretHighlight";
        this.turretHighlight.AddComponent<MeshFilter>();
        this.turretHighlight.GetComponent<MeshFilter>().mesh = sphereObjMesh;
        this.turretHighlight.AddComponent<MeshRenderer>();
        this.turretHighlight.GetComponent<MeshRenderer>().material = this.__highlight;
        this.turretHighlight.transform.localScale = new Vector3(1, 0, 1);


        this.epath = new List<Vector3>();

        int maxH = CreateMap();

        pathObj = new GameObject();
        pathObj.transform.parent = this.transform;
        pathObj.name = "Path";
        // pathObj.AddComponent<MeshCollider>();
        CreatePath();
        // pathObj.GetComponent<MeshRenderer>().material = __path;

        GameObject g = new GameObject();
        g.name = "Ground holder";
        g.transform.parent = this.transform;
        for (int i = 0; i < this.map.GetLength(1); i++)
        {
            GameObject ground = new GameObject();
            ground.transform.parent = g.transform;
            ground.name = $"Ground: {i}";
            ground.transform.position = new Vector3(0, i, 0);
            ground.tag = "Ground";
            ground.layer = 3;

            int[,,] temp = new int[worldWidth, 1, worldDepth];

            for (int x = 0; x < worldWidth; x++)
            {
                for (int z = 0; z < worldDepth; z++)
                {
                    temp[x, 0, z] = this.map[x, i, z];
                }
            }
            Material mat = new Material(__material);
            // mat.Lerp(g1, g2, i * (1 / maxH));
            mat.color = Color.Lerp(g1.color, g2.color, i * (1f / maxH));
            CreateMesh(ground, temp, mat);

            ground.AddComponent<MeshCollider>();
            ground.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            ground.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            ground.GetComponent<MeshCollider>().sharedMesh = ground.GetComponent<MeshFilter>().mesh;

            if (ground.transform.childCount == 0)
            {
                Destroy(ground);
                break;
            }
        }

        GameObject homebase = GameObject.Instantiate(this.homebaseObj, Vector3.zero, Quaternion.identity);
        homebase.name = homebase.name.Replace("(Clone)", "");
        homebase.transform.parent = this.transform;
        homebase.transform.position = eMovement[eMovement.Count - 1] + Vector3.one * 0.5f;

        enemyObj = new GameObject();
        enemyObj.transform.parent = this.transform;
        enemyObj.name = "Enemies";

        walls = new GameObject();
        walls.transform.parent = this.transform;
        walls.name = "Walls";
        walls.layer = LayerMask.NameToLayer("Walls");
        walls.AddComponent<MeshFilter>();
        walls.AddComponent<MeshCollider>();


        this.buildingObj = new GameObject();
        this.buildingObj.transform.name = "Buildings";
        this.buildingObj.transform.parent = this.transform;

        // CreateVerts();
        // CreateTris();
        // this.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        // this.mesh.RecalculateBounds();
        // MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        // meshCollider.sharedMesh = mesh;

        InvokeRepeating("look", 0f, 0.033f);
        // updateMesh();
        this.cam.transform.position = new Vector3(this.worldWidth / 2, 10, this.worldDepth / 2);
        this.cam.GetComponent<Camera>().orthographicSize = 30;


        GameObject o = GameObject.Find("Text");
        this.startpos = GameObject.Instantiate(o, this.start + Vector3.one * 0.5f, Quaternion.identity);
        this.startpos.transform.name = "StartPos";
        this.startpos.GetComponent<TextMeshProUGUI>().fontSize = 30;
        this.startpos.transform.SetParent(GameObject.Find("Canvas").transform);
        this.startpos.GetComponent<TextMeshProUGUI>().text = "Start";

        this.endpos = GameObject.Instantiate(o, this.end + Vector3.one * 0.5f, Quaternion.identity);
        this.endpos.transform.name = "Endpos";
        this.endpos.GetComponent<TextMeshProUGUI>().fontSize = 30;
        this.endpos.transform.SetParent(GameObject.Find("Canvas").transform);
        this.endpos.GetComponent<TextMeshProUGUI>().text = "End";

        for (int i = 0; i < this.hotBarItems.Count; i++)
        {
            this.hotBarItems[i].transform.GetChild(0).gameObject.SetActive(false);
            //this.hotBarItems[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Cost: " + this.costs[i];
            this.hotBarItems[i].transform.Find("Info").transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Cost: " + this.costs[i];

            if (i > 0)
            {
                this.hotBarItems[i].transform.Find("Info").transform.Find("Damage").GetComponent<TextMeshProUGUI>().text = "Damage: " + this.turretStats[i].damage;
                this.hotBarItems[i].transform.Find("Info").transform.Find("Cooldown").GetComponent<TextMeshProUGUI>().text = "Cooldown: " + this.turretStats[i].attackInterval;
            }
        }

        this.moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        this.fpsText = GameObject.Find("fpsText").GetComponent<TextMeshProUGUI>();
        this.controlsText = GameObject.Find("Controls").GetComponent<TextMeshProUGUI>();
        this.waveText = GameObject.Find("WaveText").GetComponent<TextMeshProUGUI>();

        this.moneyText.text = "Gold: " + this.money;
        this.waveText.text = this.wave + " :Wave";



        GameObject portal = GameObject.Instantiate(this.portal, this.eMovement[0] + Vector3.one * 0.5f + Vector3.up * 2f, Quaternion.identity);
        portal.transform.rotation = Quaternion.Euler(0, 45, 0);
        portal.transform.localScale = Vector3.one * 2f;

        InvokeRepeating("UpdateFPS", 1f, 0.5f);

        GameObject scenery = new GameObject();
        scenery.transform.parent = this.transform;
        scenery.name = "Scenery";

        for (int treeX = 0; treeX < this.worldWidth / 10 + 2; treeX++)
        {
            for (int treeZ = 0; treeZ < this.worldDepth / 10 + 2; treeZ++)
            {
                GameObject tree = GameObject.Instantiate(this.treeObj, new Vector3(treeX * 20, -80, treeZ * 15) + new Vector3(-20, 0, -15), Quaternion.identity);
                tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                tree.transform.parent = scenery.transform;
            }
        }

        CreateMeshFromChildren(scenery, this.treeObj.GetComponent<MeshRenderer>().sharedMaterial);

        this.initialised = true;
    }

    /* ================================================================================================================================ */

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (initialised)
        {

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (this.controlsText.gameObject.activeSelf)
                {
                    this.controlsText.gameObject.SetActive(false);
                }
                else
                {
                    this.controlsText.gameObject.SetActive(true);
                }
            }

            this.startpos.transform.position = this.cam.GetComponent<Camera>().WorldToScreenPoint(this.start + Vector3.one * 0.5f);
            this.endpos.transform.position = this.cam.GetComponent<Camera>().WorldToScreenPoint(this.end + Vector3.one * 0.5f);

            for (int i = 0; i < hotBarItems.Count; i++)
            {
                if (50f > Vector2.Distance(this.hotBarItems[i].transform.position, Input.mousePosition))
                {
                    this.hotBarItems[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    this.hotBarItems[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }


            int prevBinding = this.currentBuilding;
            for (int k = ((int)KeyCode.Alpha0); k < ((int)KeyCode.Alpha9); k++)
            {
                if (Input.GetKeyDown((KeyCode)k))
                {
                    this.currentBuilding = k - 48;
                    Debug.Log(this.currentBuilding);
                }
            }

            // hotbar logic
            for (int i = 0; i < this.hotBarItems.Count; i++)
            {
                if (i == this.currentBuilding)
                {
                    this.hotBarItems[i].transform.Find("HotbarItem").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                }
                else
                {
                    this.hotBarItems[i].transform.Find("HotbarItem").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }

            if (Input.GetMouseButtonDown(0) && this.isBuilding)
            {
                Ray ray = this.cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                {
                    Debug.Log("placing");
                    int x = (int)hit.point.x, y = (int)Mathf.Round(hit.point.y), z = (int)hit.point.z;
                    // // Debug.Log(hit.point);
                    // // Debug.Log(new Vector3(x, y, z));

                    if (y >= wallmap.GetLength(1)) return;

                    if (this.money >= this.costs[this.currentBuilding])
                    {
                        if (this.currentBuilding == 0)
                        {
                            StartCoroutine(placeWall(x, y, z));
                        }
                        else
                        {
                            if (hit.transform.parent.name == "Walls")
                            {
                                GameObject obj = Instantiate(this.buildings[this.currentBuilding], Vector3.zero, Quaternion.identity);
                                obj.transform.parent = this.buildingObj.transform;
                                obj.layer = LayerMask.NameToLayer("Walls");
                                Turret t = obj.AddComponent<Turret>();

                                this.turretStats[this.currentBuilding].towerHeadTransform = obj.transform.GetChild(1).transform;
                                t._init_(this.turretStats[this.currentBuilding]);

                                // CreateMeshFromChildren(obj.transform.GetChild(0).gameObject, __material);
                                CreateMeshFromChildren(obj, __material);
                                obj.GetComponent<MeshRenderer>().enabled = false;
                                obj.transform.GetChild(0).gameObject.SetActive(true);
                                obj.transform.GetChild(1).gameObject.SetActive(true);

                                obj.AddComponent<MeshCollider>();
                                obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;

                                obj.transform.position = new Vector3(x + 0.5f + 10f, y + 10, z + 0.5f + 10f);

                                StartCoroutine("moveToPosition", new object[] { obj, new Vector3(x + 0.5f, y, z + 0.5f) });
                                // moveToPosition(obj, new Vector3(x + 0.5f, y, z + 0.5f));

                                // if (obj.GetComponent<MeshCollider>() == null) obj.AddComponent<MeshCollider>();
                                // obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                                // obj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                                // obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;

                                this.money -= this.costs[this.currentBuilding];
                            }
                        }
                    }

                    this.moneyText.text = "Gold: " + this.money;

                    /*
                    wallmap[x, y, z] = 1;

                    // CreatePos(x, y, z, wallmap, walls);
                    // CreateMeshFromChildren(walls, __wall);

                    pathObj.GetComponent<MeshFilter>().mesh.Clear();

                    foreach (Transform t in pathObj.transform)
                    {
                        Destroy(t.gameObject);
                    }

                    CreatePath();

                    if (epath.Count > 1)
                    {
                        GameObject obj = CreatePos(x, y, z, wallmap, walls);
                        obj.layer = LayerMask.NameToLayer("Walls");
                        obj.SetActive(true);
                        if (obj.GetComponent<MeshRenderer>() == null) obj.AddComponent<MeshRenderer>();
                        if (obj.GetComponent<MeshCollider>() == null) obj.AddComponent<MeshCollider>();
                        obj.GetComponent<MeshRenderer>().material = __wall;

                        // CreateMeshFromChildren(walls, __wall);

                        obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        obj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                        obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
                    }
                    else
                    {
                        wallmap[x, y, z] = 0;

                        pathObj.GetComponent<MeshFilter>().mesh.Clear();

                        foreach (Transform t in pathObj.transform)
                        {
                            Destroy(t.gameObject);
                        }

                        CreatePath();
                    }
                    */
                }
            }
            else if (Input.GetMouseButtonDown(1) && this.isBuilding)
            {
                Ray ray = this.cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Walls")))
                {
                    Debug.Log("removing");
                    int x = (int)hit.point.x, y = (int)Mathf.Round(hit.point.y), z = (int)hit.point.z;
                    // Debug.Log(hit.point);
                    // Debug.Log(new Vector3(x, y, z));

                    if (hit.transform.parent.name == "Walls")
                    {
                        StartCoroutine(removeWall(x, y, z, hit));
                    }
                    else if (hit.transform.parent.name == "Buildings")
                    {
                        Destroy(hit.transform.gameObject);
                        string name = hit.transform.name.Replace("(Clone)", "");
                        Debug.Log(name);
                        int num = -1;
                        for (int i = 0; i < this.buildings.Count; i++)
                        {
                            if (this.buildings[i].name == name)
                            {
                                num = i;
                                break;
                            }
                        }
                        this.money += this.costs[num];
                    }

                    this.moneyText.text = "Gold: " + this.money;


                    /*
                    wallmap[x, y - 1, z] = 0;
                    // Debug.Log(hit.transform.name);
                    // CreatePos(x, y, z, wallmap, walls);
                    Destroy(hit.transform.gameObject);

                    // CreateMeshFromChildren(walls, __wall);

                    // walls.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                    // walls.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                    // walls.GetComponent<MeshCollider>().sharedMesh = walls.GetComponent<MeshFilter>().mesh;

                    pathObj.GetComponent<MeshFilter>().mesh.Clear();

                    foreach (Transform t in pathObj.transform)
                    {
                        Destroy(t.gameObject);
                    }

                    CreatePath();
                    */
                }
            }

            if (Input.GetKeyDown(KeyCode.N) && this.isBuilding)
            {
                StartCoroutine(startWave());
            }
            if (Input.GetKeyDown(KeyCode.F)) { this.fastForward = !this.fastForward; }
            Time.timeScale = (this.fastForward) ? 4f : 1f;

            Vector3 movement = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) movement += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) movement += Vector3.back;
            if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
            if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

            // if (Input.GetKey(KeyCode.Space)) movement += Vector3.up;
            // if (Input.GetKey(KeyCode.LeftShift)) movement += Vector3.down;
            // movement += -Input.mouseScrollDelta.y * Vector3.up * 30f;
            this.cam.GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * 1f;
            this.cam.GetComponent<Camera>().orthographicSize = (this.cam.GetComponent<Camera>().orthographicSize < 1) ? 1 : this.cam.GetComponent<Camera>().orthographicSize;
            this.cam.GetComponent<Camera>().orthographicSize = (this.cam.GetComponent<Camera>().orthographicSize > 20) ? 20 : this.cam.GetComponent<Camera>().orthographicSize;

            this.cam.transform.position += movement * 4f * Time.deltaTime;

            Vector3 pos = this.cam.transform.position;

            if (pos.x < 0) { pos.x = 0f; }
            if (pos.z < 0) { pos.z = 0f; }
            if (pos.x > this.worldWidth) { pos.x = this.worldWidth; }
            if (pos.z > this.worldDepth) { pos.z = this.worldDepth; }

            this.cam.transform.position = pos;
        }
    }

    /* ================================================================================================================================ */

    IEnumerator startWave()
    {
        this.isBuilding = false;
        // int numEnemies = (int)Mathf.Lerp(5, 100, this.wave * (1f / Mathf.Max(100, this.wave)));
        int numEnemies = Mathf.RoundToInt(10 * Mathf.Pow(this.wave, 0.3f));
        // int health = (int)Mathf.Lerp(10, 1000, this.wave * (1f / Mathf.Max(1000, this.wave)));
        int health = Mathf.RoundToInt(Mathf.Pow(this.wave, 0.95f) + 9);
        // float speed = Mathf.Lerp(1, 5, this.wave * (1f / Mathf.Max(200, this.wave)));
        float speed = Mathf.Min(Mathf.Pow(this.wave, 0.23f), 5f);

        this.edata.speed = speed;
        this.edata.hp = health;

        // for (int i = 0; i < numEnemies; i++)
        // {
        //     spawnEnemy();
        //     yield return new WaitForSecondsRealtime(1f);
        // }
        // this.GetComponent<EnemyManager>().run();
        // InvokeRepeating("spawnEnemy", 0f, 2f);
        for (int i = 0; i < numEnemies; i++)
        {
            spawnEnemy(i);
        }
        yield return new WaitForSecondsRealtime(numEnemies);
        // CancelInvoke("spawnEnemy");
        InvokeRepeating("checkEnemy", 0f, 1f);
        this.waveText.text = this.wave + " :Wave";
        this.wave++;
    }

    void spawnEnemy(int i)
    {
        Enemy e = GameObject.Instantiate(this.enemy, new Vector3(1f, -100000000f, 1f), Quaternion.identity);
        e.transform.parent = this.enemyObj.transform;
        e.transform.tag = "Enemy";
        StartCoroutine(e._init_(this.edata, this.eMovement, i));
        // StartCoroutine(e.init(this.eMovement, this.cam.GetComponent<Camera>(), health: 10, speed: 1f, i));
    }

    void checkEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            this.isBuilding = true;
            CancelInvoke("checkEnemy");
        }
        // else
        // {
        //     yield return new WaitForSecondsRealtime(1f);
        // }
    }

    /* ================================================================================================================================ */

    IEnumerator placeWall(int x, int y, int z)
    {
        wallmap[x, y, z] = 1;
        wallmap[x, 0, z] = 1;

        pathObj.GetComponent<MeshFilter>().mesh.Clear();

        foreach (Transform t in pathObj.transform)
        {
            Destroy(t.gameObject);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        CreatePath();

        // yield return new WaitForSecondsRealtime(0.1f);

        if (epath.Count > 1)
        {
            GameObject obj = CreatePos(x, y, z, wallmap, walls);
            obj.layer = LayerMask.NameToLayer("Walls");
            obj.SetActive(true);
            if (obj.GetComponent<MeshRenderer>() == null) obj.AddComponent<MeshRenderer>();
            if (obj.GetComponent<MeshCollider>() == null) obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshRenderer>().material = __wall;

            wallmap[x, y, z] = 0;


            // CreateMeshFromChildren(walls, __wall);

            obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            obj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;

            this.money -= this.costs[this.currentBuilding];
            this.moneyText.text = "Gold: " + this.money;
        }
        else
        {
            wallmap[x, 0, z] = 0;

            pathObj.GetComponent<MeshFilter>().mesh.Clear();

            foreach (Transform t in pathObj.transform)
            {
                Destroy(t.gameObject);
            }

            yield return new WaitForSecondsRealtime(0.1f);

            CreatePath();
        }
    }

    IEnumerator removeWall(int x, int y, int z, RaycastHit hit)
    {
        this.money += this.costs[0];
        this.moneyText.text = "Gold: " + this.money;

        wallmap[x, 0, z] = 0;
        // Debug.Log(hit.transform.name);
        // CreatePos(x, y, z, wallmap, walls);
        Destroy(hit.transform.gameObject);

        // CreateMeshFromChildren(walls, __wall);

        // walls.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        // walls.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        // walls.GetComponent<MeshCollider>().sharedMesh = walls.GetComponent<MeshFilter>().mesh;

        pathObj.GetComponent<MeshFilter>().mesh.Clear();

        foreach (Transform t in pathObj.transform)
        {
            Destroy(t.gameObject);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        CreatePath();
    }

    public void addMoney(int amount)
    {
        this.money += amount;
        this.moneyText.text = "Gold: " + this.money;
    }

    /* ================================================================================================================================ */

    void look()
    {
        Ray ray = this.cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            int x = (int)hit.point.x, y = (int)Mathf.Round(hit.point.y), z = (int)hit.point.z;
            Vector3 pos = new Vector3(x, y, z);
            // Debug.Log(new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z));
            this.highlight.transform.position = pos;

            if (hit.transform.parent.name == "Buildings")
            {
                this.turretHighlight.SetActive(true);
                float r = hit.transform.GetComponent<Turret>().distance;
                this.turretHighlight.transform.position = pos + Vector3.one * 0.5f;
                this.turretHighlight.transform.localScale = new Vector3(2 * r, 1, 2 * r);
            }
            else
            {
                this.turretHighlight.SetActive(false);
            }
        }
    }

    /* ================================================================================================================================ */

    int CreateMap()
    {
        noiseMap = Noise.GenerateNoiseMap(worldWidth, worldDepth, seed, scale, octaves, persistance, lacunarity, offset);
        float max = 0;
        for (int x = 0; x < this.worldWidth; x++)
        {
            for (int z = 0; z < this.worldDepth; z++)
            {
                noiseMap[x, z] *= heightMultiplier;
                if (noiseMap[x, z] > max) max = noiseMap[x, z];
                for (int y = 0; y < this.worldHeight; y++)
                {
                    map[x, y, z] = (y <= noiseMap[x, z] * heightMultiplier) ? 1 : 0;
                }
            }
        }
        return Mathf.RoundToInt(max * 2);
    }

    /* ================================================================================================================================ */

    void CreatePath()
    {
        // this.path = new int[(worldWidth), (worldHeight), (worldDepth)];
        // epath = new List<Vector3>();


        eMovement = new List<Vector3>();

        epath = CalcPath(start, end);

        // yield return new WaitForSecondsRealtime(0.1f);

        // Debug.Log(epath.Count);
        for (int x = 0; x < this.worldWidth; x++)
        {
            for (int z = 0; z < this.worldDepth; z++)
            {
                // path[x, z] = (epath.Contains(new Vector3(x, 0, z))) ? 1 : 0;

                for (int y = 0; y < this.worldHeight; y++)
                {
                    // path[x, y, z] = 0;
                    path[x, y, z] = (epath.Contains(new Vector3(x, 0, z))) ? 1 : 0;
                    path[x, y, z] = (map[x, y, z] == 1) ? 0 : path[x, y, z];

                    if (path[x, y, z] == 1 && path[x, y - 1, z] == 0) break;
                }
            }
        }

        foreach (Vector3 v in epath)
        {
            for (int y = 0; y < this.worldHeight; y++)
            {
                if (path[(int)v.x, y, (int)v.z] == 1)
                {
                    eMovement.Add(new Vector3(v.x, y, v.z));
                }
            }
        }

        List<Vector3> tmp = new List<Vector3>();
        foreach (Vector3 v in eMovement)
        {
            tmp.Add(v);
            int index = eMovement.IndexOf(v);
            if (index + 1 < eMovement.Count && v.y != eMovement[index + 1].y)
            {
                Debug.Log(v.y + " : " + eMovement[index + 1].y);
                if (v.y > eMovement[index + 1].y)
                {
                    tmp.Add(new Vector3(eMovement[index + 1].x, eMovement[index + 1].y + 1, eMovement[index + 1].z));
                    // eMovement.Add(new Vector3(epath[index + 1].x, epath[index + 1].y + 1, epath[index + 1].z));
                }
                else
                {
                    tmp.Add(new Vector3(v.x, v.y + 1, v.z));
                    // eMovement.Add(new Vector3(v.x, v.y + 1, v.z));
                }
            }
        }
        this.eMovement = new List<Vector3>(tmp);

        // yield return new WaitForSecondsRealtime(0.1f);

        CreateMesh(pathObj, path, __path);
        // CreateMeshFromChildren(pathObj, __path);

    }

    List<Vector3> CalcPath(Vector3 s, Vector3 e)
    {
        List<Vector3> openSet = new List<Vector3>() { s };
        List<Vector3> closedSet = new List<Vector3>() { };
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, int> gScore = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> fScore = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> dScore = new Dictionary<Vector3, int>();
        Dictionary<List<Vector3>, int> scores = new Dictionary<List<Vector3>, int>();

        for (int x = 0; x < this.worldWidth; x++)
        {
            for (int z = 0; z < this.worldDepth; z++)
            {
                gScore[new Vector3(x, 0, z)] = int.MaxValue;
                fScore[new Vector3(x, 0, z)] = int.MaxValue;

                for (int y = 0; y < this.worldHeight; y++)
                {
                    if (wallmap[x, 0, z] == 1) closedSet.Add(new Vector3(x, y, z));
                    // int wallheight = wallmap[x, 0, z];
                    // dScore[new Vector3(x, 0, z)] = (y <= noiseMap[x, z] * heightMultiplier) ? 1 : 0;
                    if (map[x, y, z] == 0)
                    {
                        dScore[new Vector3(x, 0, z)] = y - 1;
                        break;
                    }
                }
            }
        }
        gScore[s] = 0;
        fScore[s] = heuristic(s, e);

        while (openSet.Count > 0)
        {
            Vector3 curr = openSet[0];
            if (curr == e)
            {
                // Debug.Log("found path");
                // return reconstructPath(cameFrom, curr);
                List<Vector3> path = reconstructPath(cameFrom, curr);
                int cost = evalPath(path, dScore);
                scores.Add(path, cost);

                openSet.RemoveAt(0);
            }
            else
            {
                closedSet.Add(openSet[0]);
                openSet.RemoveAt(0);

                List<Vector3> neighbours = new List<Vector3>();
                if (curr.x + 1 < this.worldWidth) { neighbours.Add(new Vector3(curr.x + 1, 0, curr.z)); }
                if (curr.z + 1 < this.worldDepth) { neighbours.Add(new Vector3(curr.x, 0, curr.z + 1)); }
                if (curr.x - 1 > 0) { neighbours.Add(new Vector3(curr.x - 1, 0, curr.z)); }
                if (curr.z - 1 > 0) { neighbours.Add(new Vector3(curr.x, 0, curr.z - 1)); }

                // if (curr.x + 1 < this.worldWidth && curr.z + 1 < this.worldWidth) { neighbours.Add(new Vector3(curr.x + 1, 0, curr.z + 1)); }
                // if (curr.x + 1 < this.worldWidth && curr.z - 1 > 0) { neighbours.Add(new Vector3(curr.x + 1, 0, curr.z - 1)); }
                // if (curr.x - 1 > 0 && curr.z + 1 < this.worldWidth) { neighbours.Add(new Vector3(curr.x - 1, 0, curr.z + 1)); }
                // if (curr.x - 1 > 0 && curr.z - 1 > 0) { neighbours.Add(new Vector3(curr.x - 1, 0, curr.z - 1)); }

                foreach (Vector3 n in neighbours)
                {
                    // for (int y = 0; y < this.worldHeight; y++)
                    // {
                    //     if (wallmap[(int)n.x, y, (int)n.z] == 1)
                    //     {
                    //         closedSet.Add(n);
                    //         break;
                    //     }
                    // }

                    if (!closedSet.Contains(n))
                    {
                        int tenScore = gScore[curr] + getCost(curr, n, dScore);

                        if (tenScore <= gScore[n])
                        {
                            cameFrom[n] = curr;
                            gScore[n] = tenScore;
                            fScore[n] = tenScore + heuristic(n, e);
                            if (!openSet.Contains(n))
                            {
                                openSet.Add(n);
                            }
                        }
                    }
                }
            }
        }
        // Debug.Log("fail");

        try
        {
            Dictionary<List<Vector3>, int> tmp = scores.OrderBy(pair => pair.Value).Take(1).ToDictionary(pair => pair.Key, pair => pair.Value);
            List<Vector3> path = tmp.First().Key;
            return path;
            // return scores.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
        catch (System.Exception)
        {
            return new List<Vector3>();
            // throw;
        }

    }

    int heuristic(Vector3 s, Vector3 e)
    {
        return (int)((Mathf.Abs(e.x) - Mathf.Abs(s.x)) + (Mathf.Abs(e.z) - Mathf.Abs(s.z)));
    }

    int getCost(Vector3 c, Vector3 n, Dictionary<Vector3, int> cMap)
    {
        int cost = 0;

        int diff = cMap[c] - cMap[n];
        if (diff == 0)
        {
            cost++;
        }
        else
        {
            cost += 2000;
        }

        return cost;
    }
    List<Vector3> reconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 curr)
    {
        List<Vector3> path = new List<Vector3>() { curr };

        while (cameFrom.ContainsKey(curr))
        {
            curr = cameFrom[curr];
            path.Insert(0, curr);
        }

        return path;
    }

    int evalPath(List<Vector3> path, Dictionary<Vector3, int> cMap)
    {
        int cost = 0;

        Vector3 start, next;
        start = path[0];
        for (int i = 1; i < path.Count; i++)
        {
            next = path[i];
            cost += getCost(start, next, cMap);
            start = next;
        }
        return cost;
    }

    /* ================================================================================================================================ */

    void CreateMesh(GameObject parent, int[,,] map, Material mat)
    {
        // Debug.Log(new Vector3(map.GetLength(0), map.GetLength(1), map.GetLength(2)));
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(2); z++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    CreatePos(x, y, z, map, parent);
                }
            }
        }
        if (parent.transform.name == "Path")
        {
            foreach (Transform t in parent.transform)
            {
                t.transform.localScale = Vector3.one * 0.3f;
            }
        }
        CreateMeshFromChildren(parent, mat);
    }

    void CreateMeshFromChildren(GameObject parent, Material mat)
    {
        CombineInstance[] comb = new CombineInstance[parent.transform.childCount];

        int c = 0;
        foreach (Transform t in parent.transform)
        {
            comb[c].mesh = t.GetComponent<MeshFilter>().mesh;
            comb[c].transform = t.transform.localToWorldMatrix;
            t.gameObject.SetActive(false);
            c++;
        }
        if (parent.name == "Path") Debug.Log(c);
        if (parent.GetComponent<MeshFilter>() == null) parent.AddComponent<MeshFilter>();
        if (parent.GetComponent<MeshRenderer>() == null) parent.AddComponent<MeshRenderer>();
        parent.GetComponent<MeshRenderer>().material = mat;
        parent.GetComponent<MeshFilter>().mesh = new Mesh();
        parent.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        parent.GetComponent<MeshFilter>().mesh.CombineMeshes(comb);
        parent.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        parent.SetActive(true);
    }

    GameObject CreatePos(int x, int y, int z, int[,,] map, GameObject parent)
    {
        // Debug.Log(new Vector3(x, y, z).ToString() + " : " + new Vector3(map.GetLength(0), map.GetLength(1), map.GetLength(2)));
        if (map[x, y, z] == 1)
        {
            Vector3 mid = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
            List<GameObject> cube = new List<GameObject>();

            GameObject obj = new GameObject();
            obj.transform.parent = parent.transform;
            obj.transform.position = mid;
            obj.transform.name = $"Pos({x},{y},{z})";


            if (x + 1 >= map.GetLength(0)) { cube.Add(CreateSide(2, mid, obj)); }
            if (y + 1 >= map.GetLength(1)) { cube.Add(CreateSide(0, mid, obj)); }
            if (z + 1 >= map.GetLength(2)) { cube.Add(CreateSide(4, mid, obj)); }
            if (x - 1 < 0) { cube.Add(CreateSide(3, mid, obj)); }
            if (y - 1 < 0) { cube.Add(CreateSide(1, mid, obj)); }
            if (z - 1 < 0) { cube.Add(CreateSide(5, mid, obj)); }

            if (x + 1 < map.GetLength(0) && map[x + 1, y, z] == 0) { cube.Add(CreateSide(2, mid, obj)); }
            if (y + 1 < map.GetLength(1) && map[x, y + 1, z] == 0) { cube.Add(CreateSide(0, mid, obj)); }
            if (z + 1 < map.GetLength(2) && map[x, y, z + 1] == 0) { cube.Add(CreateSide(4, mid, obj)); }
            if (x - 1 >= 0 && map[x - 1, y, z] == 0) { cube.Add(CreateSide(3, mid, obj)); }
            if (y - 1 >= 0 && map[x, y - 1, z] == 0) { cube.Add(CreateSide(1, mid, obj)); }
            if (z - 1 >= 0 && map[x, y, z - 1] == 0) { cube.Add(CreateSide(5, mid, obj)); }

            // CreateCube(cube, parent);
            CreateMeshFromChildren(obj, __material);
            return obj;
        }
        return null;
    }

    // void CreateCube(List<GameObject> cube, GameObject obj)
    // {
    //     CombineInstance[] cubeComb = new CombineInstance[cube.Count];

    //     int c = 0;
    //     foreach (GameObject o in cube)
    //     {

    //         o.transform.parent = obj.transform;
    //         o.transform.position = Vector3.zero;

    //         cubeComb[c].mesh = o.GetComponent<MeshFilter>().mesh;
    //         cubeComb[c].transform = o.transform.localToWorldMatrix;
    //         o.SetActive(false);
    //         c++;
    //     }

    //     obj.GetComponent<MeshFilter>().mesh.CombineMeshes(cubeComb);
    //     obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    //     obj.SetActive(true);
    // }

    GameObject CreateSide(int side, Vector3 middle, GameObject p)
    {
        float o = 0.5f;
        Vector3[] vertices = new Vector3[] {
                new Vector3( o,  o,  o), // 0
                new Vector3( o,  o, -o), // 1
                new Vector3(-o,  o, -o), // 2
                new Vector3(-o,  o,  o), // 3

                new Vector3( o, -o,  o), // 4
                new Vector3( o, -o, -o), // 5
                new Vector3(-o, -o, -o), // 6
                new Vector3(-o, -o,  o)  // 7
            };
        int[][] tris = new int[][] {
                new int[]{ 0, 1, 2, 0, 2, 3 }, // +y // 0
                new int[]{ 4, 6, 5, 4, 7, 6 }, // -y // 1
                new int[]{ 0, 4, 5, 0, 5, 1 }, // +x // 2
                new int[]{ 2, 6, 7, 2, 7, 3 }, // -x // 3
                new int[]{ 0, 7, 4, 0, 3, 7 }, // +z // 4
                new int[]{ 1, 5, 6, 1, 6, 2 }  // -z // 5
            };

        Vector3[] mVerts = new Vector3[] { };
        int[] mTris = new int[] { };

        mVerts = mVerts.Concat(vertices).ToArray();
        mTris = mTris.Concat(tris[side]).ToArray();

        Mesh m = new Mesh();
        m.vertices = mVerts;
        m.triangles = mTris;

        GameObject obj = new GameObject();
        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshFilter>().mesh = m;
        // obj.transform.position = t;
        obj.transform.parent = p.transform;
        obj.name = $"Pos({middle.x},{middle.y},{middle.z})";

        return obj;
    }


    /* ================================================================================================================================ */

    private void UpdateFPS()
    {
        this.fpsText.text = Mathf.Round(Time.frameCount / Time.time) + " :FPS";
    }

    /* ================================================================================================================================ */

    private IEnumerator moveToPosition(object[] parameters)
    {
        GameObject obj = (GameObject)parameters[0];
        Vector3 pos = (Vector3)parameters[1];

        while (Vector3.Distance(obj.transform.position, pos) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, pos, 1f);
            yield return new WaitForSecondsRealtime(0.013f);
        }

    }

    /* ================================================================================================================================ */

    /*
        void updateMesh()
        {
            ClearChildren();
            this.GetComponent<MeshFilter>().mesh.Clear();
            // CreateVerts();
            CreateTris();
            this.mesh.RecalculateBounds();
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        

        void ClearChildren()
        {
            foreach (Transform t in this.transform)
            {
                Destroy(t.gameObject);
            }
        }

        void CreateVerts()
        {


            // Vector3 start, end;
            // do
            // {
            //     start = new Vector3(Random.Range(5, worldWidth - 5), 0, Random.Range(5, worldDepth - 5));
            //     end = new Vector3(Random.Range(5, worldWidth - 5), 0, Random.Range(5, worldDepth - 5));
            // } while (Mathf.Abs(Vector3.Distance(start, end)) < DistanceApart);



            // List<Vector3> pathMap = CalcPath(start, end);


            // foreach (Vector3 p in pathMap)
            // {
            //     noiseMap[(int)p.x, (int)p.z] = 1.1f;
            //     // for (int x = (int)p.x - 1; x <= (int)p.x + 1; x++)
            //     // {
            //     //     for (int z = (int)p.z - 1; z <= (int)p.z + 1; z++)
            //     //     {
            //     //         noiseMap[x, z] = 1.5f;
            //     //     }
            //     // }
            // }

            // noiseMap[(int)start.x, (int)start.z] += 1;
            // noiseMap[(int)end.x, (int)end.z] += 1;

            for (int x = 0; x <= worldWidth; x++)
            {
                for (int z = 0; z <= worldDepth; z++)
                {
                    // noiseMap[x, z] += pathMap[x, z];
                    for (int y = 0; y <= worldHeight; y++)
                    {
                        vertices[x, y, z] = new Vector3(x, y, z);

                        map[x, y, z] = (y <= noiseMap[x, z] * heightMultiplier) ? 1 : 0;
                    }
                }
            }
        }

        void CreateTris()
        {
            List<GameObject> objs = new List<GameObject>();

            for (int x = 0; x < worldWidth; x++)
            {
                for (int z = 0; z < worldDepth; z++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        CreatePos(x, y, z);
                        // if (map[x, y, z] == 1)
                        // {
                        //     Vector3 mid = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);

                        //     List<GameObject> cube = new List<GameObject>();

                        //     if (x + 1 >= worldWidth) { cube.Add(CreateSide(2, mid)); }
                        //     if (y + 1 >= worldHeight) { cube.Add(CreateSide(0, mid)); }
                        //     if (z + 1 >= worldDepth) { cube.Add(CreateSide(4, mid)); }
                        //     if (x - 1 < 0) { cube.Add(CreateSide(3, mid)); }
                        //     if (y - 1 < 0) { cube.Add(CreateSide(1, mid)); }
                        //     if (z - 1 < 0) { cube.Add(CreateSide(5, mid)); }


                        //     if (x + 1 < worldWidth && map[x + 1, y, z] == 0) { cube.Add(CreateSide(2, mid)); }
                        //     if (y + 1 < worldHeight && map[x, y + 1, z] == 0) { cube.Add(CreateSide(0, mid)); }
                        //     if (z + 1 < worldDepth && map[x, y, z + 1] == 0) { cube.Add(CreateSide(4, mid)); }
                        //     if (x - 1 >= 0 && map[x - 1, y, z] == 0) { cube.Add(CreateSide(3, mid)); }
                        //     if (y - 1 >= 0 && map[x, y - 1, z] == 0) { cube.Add(CreateSide(1, mid)); }
                        //     if (z - 1 >= 0 && map[x, y, z - 1] == 0) { cube.Add(CreateSide(5, mid)); }

                        //     GameObject obj = new GameObject();
                        //     obj.AddComponent<MeshFilter>();
                        //     obj.GetComponent<MeshFilter>().mesh = new Mesh();
                        //     obj.transform.parent = this.transform;
                        //     obj.transform.position = mid;
                        //     obj.name = $"Pos({mid.x},{mid.y},{mid.z})";

                        //     CreateCube(cube, obj);

                        //     objs.Add(obj);
                        // }
                    }
                }
            }

            foreach (Transform t in this.transform)
            {
                objs.Add(t.gameObject);
            }

            CreateMesh(objs);
        }

        

        void CreateCube(List<GameObject> cube, GameObject obj)
        {
            CombineInstance[] cubeComb = new CombineInstance[cube.Count];

            int c = 0;
            foreach (GameObject o in cube)
            {

                o.transform.parent = obj.transform;
                o.transform.position = Vector3.zero;

                cubeComb[c].mesh = o.GetComponent<MeshFilter>().mesh;
                cubeComb[c].transform = o.transform.localToWorldMatrix;
                o.SetActive(false);
                c++;
            }

            obj.GetComponent<MeshFilter>().mesh.CombineMeshes(cubeComb);
            obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            obj.SetActive(true);
        }

        void CreateMesh(List<GameObject> objs)
        {
            CombineInstance[] combine = new CombineInstance[objs.Count];

            int i = 0;
            foreach (GameObject o in objs)
            {
                combine[i].mesh = o.GetComponent<MeshFilter>().mesh;
                combine[i].transform = o.transform.localToWorldMatrix;
                o.SetActive(false);
                i++;
            }

            transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            transform.gameObject.SetActive(true);

            // this.mesh.RecalculateBounds();
            // this.mesh.RecalculateNormals();
        }

        

        
        

        // private void OnDrawGizmos()
        // {
        //     if (vertices == null)
        //         return;

        //     for (int x = 0; x < worldWidth; x++)
        //     {
        //         for (int z = 0; z < worldDepth; z++)
        //         {
        //             for (int y = 0; y < worldHeight; y++)
        //             {
        //                 if (map[x, y, z] == 1)
        //                 {
        //                     Gizmos.DrawSphere(vertices[x, y, z], 0.1f);
        //                 }
        //             }
        //         }
        //     }
        // }
    */
}
