using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(MeshFilter))]
[ExecuteInEditMode]
public class WaterMesh : MonoBehaviour {

    // Use this for initialization
    private MeshFilter _meshfilter;

    public float _step = 0.1f;
    public int _resolution = 128;
	void Start () {
        _meshfilter = gameObject.GetComponent<MeshFilter>();
        _meshfilter.mesh = getWaterMesh();
		
	}
	Mesh getWaterMesh()
    {
        Mesh _mesh = new Mesh();
        List<Vector3> _vertices = new List<Vector3>();
        List<Vector2> _uv = new List<Vector2>();
        List<int> _triangles = new List<int>();
        float left = - _step * _resolution / 2;
        for(int i = 0;i<_resolution;++i)
        {
            for(int j = 0;j<_resolution;++j)
            {
                _vertices.Add(new Vector3(left+i*_step , 0.0f , left + j * _step));
                _uv.Add(new Vector2(1.0f*i / _resolution , 1.0f*j / _resolution));

                if(i <_resolution -1 && j<_resolution -1)
                {
                    int index = i * _resolution + j;
                    _triangles.Add(index);  //顺时针
                    _triangles.Add(index + 1);
                    _triangles.Add(index + _resolution);

                    _triangles.Add(index + 1);
                    _triangles.Add(index + _resolution +1);
                    _triangles.Add(index + _resolution);

                }

            }

        }
        _mesh.vertices = _vertices.ToArray();
        _mesh.SetIndices(_triangles.ToArray(), MeshTopology.Triangles, 0);
        _mesh.uv = _uv.ToArray();
       // _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();


        return _mesh;

    }
	// Update is called once per frame
	void Update () {
		
	}
}
