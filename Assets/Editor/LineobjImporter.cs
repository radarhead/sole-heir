using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

[ScriptedImporter(1, "lineobj")]
public class LineobjImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        int lastSlash = ctx.assetPath.LastIndexOf('/');
        int lastDot = ctx.assetPath.LastIndexOf('.');
        string assetName = ctx.assetPath;

        GameObject mainAsset = new GameObject();

        Dictionary<string, List<string[]>> obj = ParseObj(ctx.assetPath);

        var lines = SplitLines(obj);
        int count = 0;
        Material material = Resources.Load<Material>("Inner Line");
        foreach (var line in lines)
        {
            count++;
            LineRenderer lr = new GameObject(string.Format("Line {0}", count)).AddComponent<LineRenderer>();
            lr.positionCount = line.Count;
            lr.useWorldSpace = false;
            lr.material = material;
            lr.widthMultiplier = 0.03f;
            lr.SetPositions(line.ToArray());
            lr.gameObject.transform.parent = mainAsset.transform;
            ctx.AddObjectToAsset(string.Format("Line {0}", count), lr.gameObject);
        }
        ctx.AddObjectToAsset("ctx", mainAsset);
        ctx.SetMainObject(mainAsset);
    }

    private List<List<Vector3>> SplitLines(Dictionary<string, List<string[]>> data)
    {
        List<string[]> verts = data["v"];
        List<string[]> edges = data["e"];

        List<List<Vector3>> output = new List<List<Vector3>>();

        int lastIdx = -1;

        for (int i = 0; i < edges.Count; i++)
        {
            int[] edge = GetEdge(edges[i]);
            Vector3 vertex1 = GetVertex(verts[edge[0]]);
            Vector3 vertex2 = GetVertex(verts[edge[1]]);

            if(lastIdx != edge[0])
            {
                output.Add(new List<Vector3>());
                output.Last().Add(vertex1);
            }
            output.Last().Add(vertex2);
            lastIdx = edge[1];
        }

        return output;
    }

    private int[] GetEdge(string[] data)
    {
        return new int[] {int.Parse(data[0])-1, int.Parse(data[1])-1};
    }

    private Vector3 GetVertex(string[] data)
    {
        return new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
    }

    private Mesh ConstructMesh(Dictionary<string, List<string[]>> data, out bool triangleSubmeshExists)
    {
        Mesh result;
        List<string[]> f = data["f"];
        List<string[]> e = data["e"];
        triangleSubmeshExists = false;
        if (f.Count > 0 && e.Count > 0)
        {
            result = new Mesh();
            result.subMeshCount = 2;
            triangleSubmeshExists = true;
        }
        else if (f.Count > 0)
        {
            result = new Mesh();
            result.subMeshCount = 1;
            triangleSubmeshExists = true;
        }
        else if (e.Count > 0)
        {
            result = new Mesh();
            result.subMeshCount = 1;
        }
        else
        {
            return null;
        }

        List<string[]> v = data["v"];
        Vector3[] vertices = new Vector3[v.Count];
        for (int i = 0; i < v.Count; i++)
        {
            string[] raw = v[i];

            float x = float.Parse(raw[0]);
            float y = float.Parse(raw[1]);
            float z = float.Parse(raw[2]);
            vertices[i] = new Vector3(x, y, z);
        }
        result.vertices = vertices;

        // subMesh 0 is like a regular mesh which uses MeshTopology.Triangles
        if (f.Count > 0)
        {
            int[] triangleIndices = new int[f.Count * 3];
            for (int i = 0; i < f.Count; i++)
            {
                string[] raw = f[i];
                string s1 = raw[0];
                string s2 = raw[1];
                string s3 = raw[2];
                if (s1.Contains("//"))
                {
                    s1 = s1.Remove(s1.IndexOf("//"));
                }
                if (s2.Contains("//"))
                {
                    s2 = s2.Remove(s2.IndexOf("//"));
                }
                if (s3.Contains("//"))
                {
                    s3 = s3.Remove(s3.IndexOf("//"));
                }
                int v1 = int.Parse(s1) - 1;
                int v2 = int.Parse(s2) - 1;
                int v3 = int.Parse(s3) - 1;
                triangleIndices[i * 3] = v1;
                triangleIndices[i * 3 + 1] = v2;
                triangleIndices[i * 3 + 2] = v3;
            }
            result.SetIndices(triangleIndices, MeshTopology.Triangles, 0, false);
            result.RecalculateNormals();
        }


        // subMesh 1 is the line mesh which uses MeshTopology.Lines
        if (e.Count > 0)
        {
            int[] edgeIndices = new int[e.Count * 2];
            for (int i = 0; i < e.Count; i++)
            {
                string[] raw = e[i];
                int v1 = int.Parse(raw[0]) - 1;
                int v2 = int.Parse(raw[1]) - 1;
                edgeIndices[i * 2] = v1;
                edgeIndices[i * 2 + 1] = v2;
            }
            if (triangleSubmeshExists)
            {
                result.SetIndices(edgeIndices, MeshTopology.Lines, 1, false);
            }
            else
            {
                result.SetIndices(edgeIndices, MeshTopology.Lines, 0, false);
            }
        }


        result.RecalculateBounds();
        return result;
    }

    /*
    Converts obj text file into json-like structure:
        {v: [], vn: [], f: [], e: []}
     */
    private Dictionary<string, List<string[]>> ParseObj(string filepath)
    {
        Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
        List<string[]> v = new List<string[]>();
        List<string[]> vn = new List<string[]>();
        List<string[]> f = new List<string[]>();
        List<string[]> e = new List<string[]>();

        using (StreamReader sr = File.OpenText(filepath))
        {
            string s = string.Empty;
            string[] line;
            while ((s = sr.ReadLine()) != null)
            {
                if (s.StartsWith("v "))
                {
                    line = s.Split(' ');
                    string[] lineData = { line[1], line[2], line[3] };
                    v.Add(lineData);
                }
                else if (s.StartsWith("vn "))
                {
                    line = s.Split(' ');
                    string[] lineData = { line[1], line[2], line[3] };
                    vn.Add(lineData);
                }
                else if (s.StartsWith("f "))
                {
                    line = s.Split(' ');
                    if (line.Length > 4)
                    {
                        Debug.LogError("Your model must be exported with triangulated faces.");
                        continue;
                    }
                    string[] lineData = { line[1], line[2], line[3] };
                    f.Add(lineData);
                }
                else if (s.StartsWith("l "))
                {
                    line = s.Split(' ');
                    string[] lineData = { line[1], line[2] };
                    e.Add(lineData);
                }
            }
        }

        result.Add("v", v);
        result.Add("vn", vn);
        result.Add("f", f);
        result.Add("e", e);
        return result;
    }

    // for debugging
    private void LogObj(Dictionary<string, List<string[]>> obj)
    {
        string result = "";
        result += "{\n";

        result += LogChild(obj, "v");
        result += LogChild(obj, "vn");
        result += LogChild(obj, "f");
        result += LogChild(obj, "e");

        result += "}";
        Debug.Log(result);
    }

    private string LogChild(Dictionary<string, List<string[]>> obj, string key)
    {
        string result = "";
        string ind = "  ";
        result += ind + key + ": [\n";
        foreach (string[] sarr in obj[key])
        {
            result += ind + ind + "[";
            foreach (string s in sarr)
            {
                result += s + ", ";
            }
            result += "]\n";
        }
        result += ind + "]\n";
        return result;
    }
}