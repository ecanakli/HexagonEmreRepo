using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //Prefabs
    [SerializeField] GameObject _hexPrefab;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] GameObject explodeParticle;

    Camera _myCam;

    //Hexagon Lists
    private List<List<Hexagon>> hexagonList = new List<List<Hexagon>>();
    private List<Hexagon> selectedHexagonList = new List<Hexagon>();
    private List<BombHexagon> bombList = new List<BombHexagon>();

    //Variables
    private int _gridWidth;
    private int _gridHeight;

    [SerializeField] int _hexagonPoint = 5;
    [SerializeField] float _rotateSpeed = 0.3f;

    private int _score = 0;
    private int _moves = 0;

    private bool _isSelected;

    public static GridManager _instance;

    //Singleton pattern to access scripts
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    //Creating grid system process
    public void CreateGrid()
    {
        _gridWidth = SettingsManager._instance._gridWidth;
        _gridHeight = SettingsManager._instance._gridHeight;

        StartCoroutine(HexagonalGridSystem());
    }

    //Creating hexagonal based system
    IEnumerator HexagonalGridSystem()
    {
        _isSelected = false;

        float vPos = Mathf.Sqrt(3);

        //Creating Hexagons by looking at gridWidth and gridHeight values

        for (int x = 0; x < _gridWidth; x++)
        {
            List<Hexagon> _vHexagons = new List<Hexagon>();

            for (int y = 0; y < _gridHeight; y++)
            {
                Vector3 newPos = new Vector3(x * 1.55f, (vPos + y) * _gridHeight * 1, 0);

                GameObject tHex = Instantiate(_hexPrefab, newPos, Quaternion.identity, transform);

                tHex.GetComponent<Hexagon>()._color = ColorManager._instance.GetStartColor(x, y);
                tHex.GetComponent<Hexagon>().SetGridPos(x, y);
                tHex.name = "Hexagon_" + x + "_" + y;
                SoundManager._instance.ReplacementS();

                _vHexagons.Add(tHex.GetComponent<Hexagon>());

                yield return new WaitForSeconds(0.03f);
            }
            hexagonList.Add(_vHexagons);
        }

        _isSelected = true;

    }

    //Scale camera positions to the Hex positions
    public void SetCameraPos()
    {
        _myCam = FindObjectOfType<Camera>();
        float x = (_gridWidth - 1) * 1.55f / 2f;
        float y = (_gridHeight - 1) + (_gridHeight / 5);
        _myCam.transform.position = new Vector3(x, y, _myCam.transform.position.z);
        _myCam.orthographicSize = _gridWidth + _gridHeight + 1.55f;
    }

    //To select hexagons by clicking
    public void SelectHexagons(Hexagon hexagon, int angle)
    {
        if (_isSelected)
        {
            SoundManager._instance.BombClickS();

            DeSelect();

            List<Hexagon> neighborsList = GetNeighbors(hexagon, angle); //Selecting hexagon group by looking at the neighbors and adding this group to the list.
            neighborsList.Add(hexagon);
            selectedHexagonList = neighborsList;

            //Show white edges on the selected hex group
            for (int i = 0; i < selectedHexagonList.Count; i++)
            {
                selectedHexagonList[i].Selected(true);
            }
        }
    }

    //Deselect hexagons
    private void DeSelect()
    {
        foreach (Hexagon hex in transform.GetComponentsInChildren<Hexagon>()) //Removing edges
        {
            hex.Selected(false);
        }
    }

    //Finds adjacent hexes in clicked places
    public List<Hexagon> GetNeighbors(Hexagon hexagon, int angle)
    {
        List<Hexagon> returnNeighbors = new List<Hexagon>();
        List<Hexagon> allNeighbors = GetAllNeighbors(hexagon);

        for (int i = 0; i < 6; i++)
        {
            int _angle = angle + i * 60 * (i % 2 == 0 ? 1 : -1);
            _angle = (_angle + 360) % 360;

            int _first = (int)_angle / 60;
            int _second = (_first + 1) % allNeighbors.Count;
            _second = _second == 0 ? 6 : _second;

            if (_angle >= _first * 60 && _angle < _second * 60)
            {
                _second %= allNeighbors.Count;

                if (allNeighbors[_first] && allNeighbors[_second])
                {
                    returnNeighbors.Add(allNeighbors[_first]);
                    returnNeighbors.Add(allNeighbors[_second]);
                    break;
                }
            }
        }

        return returnNeighbors;
    }

    //Looking at the neighbors of the selected hexagon group
    public List<Hexagon> GetAllNeighbors(Hexagon hexagon)
    {
        List<Hexagon> neighborsList = new List<Hexagon>();

        Vector2[] _tNeighborsList = new Vector2[6];

        _tNeighborsList[0] = new Vector2(hexagon._xValue, hexagon._yValue + 1); //up
        _tNeighborsList[1] = new Vector2(hexagon._xValue + 1, hexagon._yValue + (hexagon._xValue % 2 == 0 ? 0 : 1)); //rightUp
        _tNeighborsList[2] = new Vector2(hexagon._xValue + 1, hexagon._yValue - (hexagon._xValue % 2 == 0 ? 1 : 0)); //rightDown
        _tNeighborsList[3] = new Vector2(hexagon._xValue, hexagon._yValue - 1);   //down
        _tNeighborsList[4] = new Vector2(hexagon._xValue - 1, hexagon._yValue - (hexagon._xValue % 2 == 0 ? 1 : 0)); //leftDown
        _tNeighborsList[5] = new Vector2(hexagon._xValue - 1, hexagon._yValue + (hexagon._xValue % 2 == 0 ? 0 : 1)); //leftUp

        for (int j = 0; j < _tNeighborsList.Length; j++)
        {
            if (_tNeighborsList[j].x >= 0 && _tNeighborsList[j].x < _gridWidth && _tNeighborsList[j].y >= 0 && _tNeighborsList[j].y < _gridHeight)
            {
                neighborsList.Add(hexagonList[(int)_tNeighborsList[j].x][(int)_tNeighborsList[j].y]);
            }
            else
            {
                neighborsList.Add(null);
            }
        }

        return neighborsList;
    }

    //The process of rotating hexes
    public void RotateHexagons(int angle)
    {
        if (_isSelected && selectedHexagonList.Count > 0)
        {
            StartCoroutine(IRotateHexagons(angle));
        }
    }

    //Rotation Process
    IEnumerator IRotateHexagons(int angle)
    {
        _isSelected = false; //To avoid select other hexagons while rotate process

        List<Hexagon> _explodedHexagonList = new List<Hexagon>();

        for (int i = 0; i < 3; i++)
        {
            SwapHexagons(angle);

            SoundManager._instance.BombRotationS();
            yield return new WaitForSeconds(_rotateSpeed);

            for (int j = 0; j < selectedHexagonList.Count; j++)
            {
                _explodedHexagonList = CheckMatch(selectedHexagonList[j]);

                if (_explodedHexagonList.Count > 0)
                {
                    break;
                }
            }

            //Deselect after the explosion
            if (_explodedHexagonList.Count > 0)
            {
                DeSelect();
                selectedHexagonList.Clear();
                break;
            }
        }

        //Rescale hexagons after the explosion
        while (_explodedHexagonList.Count > 0)
        {
            for (int i = 0; i < _explodedHexagonList.Count; i++)
            {
                if (!FillGrid(_explodedHexagonList[i]))
                {
                    ResetHexagon(_explodedHexagonList[i]);
                    _explodedHexagonList.Remove(_explodedHexagonList[i]);
                }
                yield return new WaitForSeconds(0.025f);
            }

            if (_explodedHexagonList.Count == 0)
            {
                yield return new WaitForSeconds(0.1f);
                _explodedHexagonList = CheckAllMatches();
            }
        }

        BombsCountdown();

        if (!CheckPlayable())
        {
            FinishGame();
        }

        _isSelected = true;
    }

    //Swapping hexagon group
    private void SwapHexagons(int angle)
    {
        Hexagon hexagon0 = selectedHexagonList[0];
        Hexagon hexagon1 = selectedHexagonList[1];
        Hexagon hexagon2 = selectedHexagonList[2];

        Vector2 t_gridPos0 = new Vector2(hexagon0._xValue, hexagon0._yValue);
        Vector2 t_gridPos1 = new Vector2(hexagon1._xValue, hexagon1._yValue);
        Vector2 t_gridPos2 = new Vector2(hexagon2._xValue, hexagon2._yValue);

        if (angle > 0)
        {
            hexagonList[(int)t_gridPos2.x][(int)t_gridPos2.y] = hexagon0;
            hexagonList[(int)t_gridPos0.x][(int)t_gridPos0.y] = hexagon1;
            hexagonList[(int)t_gridPos1.x][(int)t_gridPos1.y] = hexagon2;

            hexagon0.SetGridPos((int)t_gridPos2.x, (int)t_gridPos2.y);
            hexagon1.SetGridPos((int)t_gridPos0.x, (int)t_gridPos0.y);
            hexagon2.SetGridPos((int)t_gridPos1.x, (int)t_gridPos1.y);
        }
        else
        {
            hexagonList[(int)t_gridPos1.x][(int)t_gridPos1.y] = hexagon0;
            hexagonList[(int)t_gridPos2.x][(int)t_gridPos2.y] = hexagon1;
            hexagonList[(int)t_gridPos0.x][(int)t_gridPos0.y] = hexagon2;

            hexagon0.SetGridPos((int)t_gridPos1.x, (int)t_gridPos1.y);
            hexagon1.SetGridPos((int)t_gridPos2.x, (int)t_gridPos2.y);
            hexagon2.SetGridPos((int)t_gridPos0.x, (int)t_gridPos0.y);
        }
    }

    //Cheking If 3 hexagon of the same color come across, explode the hexagon group
    public List<Hexagon> CheckMatch(Hexagon hexagon)
    {
        List<Hexagon> explodedHexagonList = new List<Hexagon>();
        List<Hexagon> allNeighborsList = GetAllNeighbors(hexagon);

        for (int j = 0; j < allNeighborsList.Count; j++)
        {
            int _second = (j + 1) % allNeighborsList.Count;
            if (allNeighborsList[j] && allNeighborsList[_second])
            {
                if (hexagon._color == allNeighborsList[j]._color && hexagon._color == allNeighborsList[_second]._color)
                {
                    explodedHexagonList.Add(hexagon);
                    explodedHexagonList.Add(allNeighborsList[j]);
                    explodedHexagonList.Add(allNeighborsList[_second]);

                    Explode(hexagon);
                    ExplosionEffect(hexagon);
                    Explode(allNeighborsList[j]);
                    Explode(allNeighborsList[_second]);

                    return explodedHexagonList;
                }
            }
        }

        return explodedHexagonList;
    }

    //Cheking all matching hexagons in the scene, If there are any explode that group
    public List<Hexagon> CheckAllMatches()
    {
        List<Hexagon> explodedHexagonList = new List<Hexagon>();

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                Hexagon hexagon = hexagonList[x][y];

                List<Hexagon> allNeighborsList = GetAllNeighbors(hexagon);

                for (int j = 0; j < allNeighborsList.Count; j++)
                {
                    int _second = (j + 1) % allNeighborsList.Count;
                    if (allNeighborsList[j] && allNeighborsList[_second])
                    {

                        if (hexagon._color == allNeighborsList[j]._color && hexagon._color == allNeighborsList[_second]._color)
                        {
                            explodedHexagonList.Add(hexagon);
                            explodedHexagonList.Add(allNeighborsList[j]);
                            explodedHexagonList.Add(allNeighborsList[_second]);

                            Explode(hexagon);
                            ExplosionEffect(hexagon);
                            Explode(allNeighborsList[j]);
                            Explode(allNeighborsList[_second]);

                            return explodedHexagonList;
                        }
                    }
                }

            }
        }

        SetMoves();

        return explodedHexagonList;
    }

    //Increase move after each successful matching
    private void SetMoves()
    {
        _moves++;
        GameManagement._instance.SetMove(_moves);
    }

    //Destroying hexagons
    private void Explode(Hexagon hexagon)
    {
        for (int i = 0; i < bombList.Count; i++)
        {
            if (bombList[i] && bombList[i] == hexagon.GetComponentInChildren<BombHexagon>())
            {
                bombList[i] = null;
                SoundManager._instance.BombRemoveS();
                Destroy(hexagon.GetComponentInChildren<BombHexagon>().gameObject);
            }
        }

        SoundManager._instance.ClusterS();

        hexagon.Selected(false);
        hexagon._color = Color.black;
        hexagon.SetSprite(false);

        CalculateScore();
    }

    //Explosion Particle Effect After The Hexagon Match
    private void ExplosionEffect(Hexagon hexagon)
    {
        Color hexColor = hexagon.GetComponent<SpriteRenderer>().color;
        ParticleSystem.MainModule settings = explodeParticle.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(hexColor);
        GameObject explodeEffect = Instantiate(explodeParticle, new Vector3(hexagon.transform.position.x, hexagon.transform.position.y, explodeParticle.transform.position.z), Quaternion.identity);
        Destroy(explodeEffect, 2f);
    }

    //Increase Score after each successful matching
    private void CalculateScore()
    {
        _score += _hexagonPoint;
        GameManagement._instance.SetScore(_score);
    }

    //Replacing destroyed hexagons with the new ones
    public void ResetHexagon(Hexagon hexagon)
    {
        hexagon.transform.position = hexagon.transform.position + new Vector3(0f, hexagon._yValue * 1.55f, 0f);
        hexagon._color = ColorManager._instance.GetHexagonColor();
        hexagon.Start();

        Bomb(hexagon);
    }

    //Filling new hexagons after the explosion
    private bool FillGrid(Hexagon hexagon)
    {
        for (int i = 1; i < _gridHeight - 1; i++)
        {
            if (hexagon._yValue + i < _gridHeight && hexagonList[hexagon._xValue][hexagon._yValue + i]._color != Color.black)
            {
                Hexagon hexagon0 = hexagonList[hexagon._xValue][hexagon._yValue];
                Hexagon hexagon1 = hexagonList[hexagon._xValue][hexagon._yValue + i];

                Vector2 gridPos0 = new Vector2(hexagon0._xValue, hexagon0._yValue);
                Vector2 gridPos1 = new Vector2(hexagon1._xValue, hexagon1._yValue);

                hexagon0.SetGridPos((int)gridPos1.x, (int)gridPos1.y, 25);
                hexagon1.SetGridPos((int)gridPos0.x, (int)gridPos0.y, 25);

                hexagonList[(int)gridPos1.x][(int)gridPos1.y] = hexagon0;
                hexagonList[(int)gridPos0.x][(int)gridPos0.y] = hexagon1;

                return true;
            }
        }
        return false;
    }

    //Checking are there any hexagon to match
    public bool CheckPlayable()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                List<Hexagon> allNeighborsList = GetAllNeighbors(hexagonList[x][y]);

                List<Hexagon> matchColorList = new List<Hexagon>();

                for (int j = 0; j < allNeighborsList.Count; j++)
                {
                    int t_second = (j + 1) % allNeighborsList.Count;
                    if (allNeighborsList[j] && allNeighborsList[t_second])
                    {
                        if (allNeighborsList[j]._color == allNeighborsList[t_second]._color)
                        {
                            matchColorList.Add(allNeighborsList[j]);
                            matchColorList.Add(allNeighborsList[t_second]);

                            allNeighborsList[j] = null;
                            allNeighborsList[t_second] = null;
                        }
                    }
                }

                for (int i = 0; i < matchColorList.Count; i++)
                {
                    for (int j = 0; j < allNeighborsList.Count; j++)
                    {
                        if (allNeighborsList[j] && allNeighborsList[j]._color == matchColorList[i]._color)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    //If the desired score is reached, place a bomb instead of the hexagon
    private void Bomb(Hexagon hexagon)
    {
        if (bombList.Count < (int)_score / SettingsManager._instance._bombAppearScore)
        {
            hexagon.SetSprite(false);

            GameObject _bomb = Instantiate(_bombPrefab, hexagon.transform.position, Quaternion.identity, hexagon.transform) as GameObject;
            _bomb.transform.localPosition += new Vector3(0.1f, 0f, 0f);
            _bomb.GetComponent<BombHexagon>().SetColor(hexagon._color);
            bombList.Add(_bomb.GetComponent<BombHexagon>());
        }
    }

    //Bomb sound after each move
    public void BombsCountdown()
    {
        for (int i = 0; i < bombList.Count; i++)
        {
            if (bombList[i])
            {
                SoundManager._instance.BombTimerS();
                bombList[i].Countdown();
            }
        }
    }

    //If the bomb is not destroyed in the specified time.
    public void BombExploded()
    {
        SoundManager._instance.BombExplodeS();
        FinishGame();
    }

    public void FinishGame()
    {
        GameManagement._instance.SetFinishScore(_score);
        GameManagement._instance.FinishGame();
    }
}