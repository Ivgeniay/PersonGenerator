using System.Collections.Generic;
using SystemFunc.Attributes;
using System.Reflection;
using Meshes.Chains;
using System.Linq;
using UnityEngine;

namespace Skillets
{
    [RequireComponent(typeof(SkilletBuilder))]
    public class MeshBuilder : MonoBehaviour
    {
        [SerializeField] private Material material; 
        [SerializeField] private MeshChainConnector chainConnector;
        
        public SkilletBuilder SkilletBuilder;
        private List<Transform> meshPointsTransforms = new();
        private IEnumerable<(Transform, FieldInfo)> bonesTransformBindedField;
        private List<(Transform, MeshChain, FieldInfo)> bonesTransformMeshChainBindedField = new();

        public void GenerateMesh()
        {
            FillBones(); 
            if (!Validation()) return;

            GameObject meshRender = new GameObject(nameof(MeshBuilder));
            CloneChildren(SkilletBuilder.transform, meshRender.transform);
            meshRender.transform.SetParent(SkilletBuilder.transform);
            MakeAtomarMeshes(); 
            CombineMesh(SkilletBuilder.transform);
            DestroyAtomarMeshes(meshRender);
        }

        private void FillBones()
        {
            if (!SkilletBuilder) SkilletBuilder = GetComponent<SkilletBuilder>();
            meshPointsTransforms.Clear();
            bonesTransformBindedField = SkilletBuilder.GetBoneTransforms();
        }
        private bool Validation()
        {
            if (bonesTransformBindedField == null || bonesTransformBindedField.Count() == 0)
            {
                Debug.LogError("No bones provided.");
                return false;
            }  
            return true;
        }
        private void CloneChildren(Transform source, Transform destinationParent = null)
        {
            foreach (Transform child in source)
            { 
                if (meshPointsTransforms.Contains(source)) continue; 

                GameObject newChild = new GameObject($"{child.name}(MeshBuilder)");

                meshPointsTransforms.Add(newChild.transform);
                var mChain = newChild.AddComponent<MeshChain>();
                mChain.Initialize(4, 0.1f); 

                var refer = bonesTransformBindedField.FirstOrDefault(x => x.Item1 == child).Item1;
                if (refer != null) bonesTransformMeshChainBindedField
                                            .Add((refer, mChain, bonesTransformBindedField
                                                                        .FirstOrDefault(x => x.Item1 == child)
                                                                        .Item2));
                 
                newChild.transform.SetParent(destinationParent); 
                newChild.transform.localPosition = child.localPosition;
                newChild.transform.localRotation = child.localRotation;
                newChild.transform.localScale = child.localScale; 

                CloneChildren(child, newChild.transform);
            } 
        }
        private void MakeAtomarMeshes()
        {
            foreach (var (bone, meshChain, field) in bonesTransformMeshChainBindedField)
            {
                ConnectionAttribute attribute = field.GetCustomAttribute<ConnectionAttribute>();
                if (attribute == null)
                {
                    Debug.Log($"No ConnectionAttribute found for {field.Name}");
                    continue;
                }
                var connectionTupple = bonesTransformMeshChainBindedField.FirstOrDefault(e => e.Item3.Name == attribute.ConnectionFieldName);
                if (connectionTupple == default)
                {
                    Debug.Log($"No connection found for {field.Name}");
                    continue;
                }

                MeshRenderer meshRenderer = meshChain.gameObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = meshChain.gameObject.AddComponent<MeshFilter>();
                meshRenderer.material = material;
                chainConnector.Connect(new List<MeshChain>() { meshChain, connectionTupple.Item2 }, meshFilter);
            }
        }
        private void CombineMesh(Transform root)
        {
            IEnumerable<MeshFilter> meshes = root.GetComponentsInChildren<MeshFilter>()
                                                .Where(mf => mf.GetComponent<MeshChain>() != null);

            gameObject.GetComponent<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshes.Count()];

            int counter = 0;
            foreach (MeshFilter item in meshes)
            {
                //combine[counter].mesh = item.mesh;
                combine[counter].mesh = item.sharedMesh;
                combine[counter].transform = item.transform.localToWorldMatrix;
                counter++;
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            MeshFilter rootMeshFilter = root.gameObject.AddComponent<MeshFilter>();
            rootMeshFilter.mesh = combinedMesh;
            rootMeshFilter.mesh.name = "CombinedMesh";
            MeshRenderer rootMeshRenderer = rootMeshFilter.gameObject.AddComponent<MeshRenderer>();
            rootMeshRenderer.material = material;
        }
        private void DestroyAtomarMeshes(GameObject meshRender)
        {
            DestroyImmediate(meshRender);
            meshPointsTransforms.Clear();
        }
        public void GenerateSkinRenerer(Transform root)
        {
            if (root is null)
            {
                Debug.LogError("Root is null");
                return;
            }

            MeshFilter meshFilter = root.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMeshRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = meshFilter.sharedMesh;

            DestroyImmediate(meshFilter);

            Transform[] bones = SkilletBuilder.GetBoneTransforms().Select(e => e.Item1).ToArray();
            Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                bindPoses[i] = CreateBindPose(bones[i], root);
            }
            skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
            SetBoneWeightForTwoNearesBone(skinnedMeshRenderer, bones);
            //SetBoneWeightForFourNearesBone(skinnedMeshRenderer, bones);

            skinnedMeshRenderer.material = material;
            skinnedMeshRenderer.bones = bones;
            skinnedMeshRenderer.rootBone = root;
            skinnedMeshRenderer.sharedMesh.RecalculateBounds();
            skinnedMeshRenderer.sharedMesh.RecalculateNormals();
            skinnedMeshRenderer.sharedMesh.RecalculateTangents();
        }
        private static void SetBoneWeightForTwoNearesBone(SkinnedMeshRenderer skinnedMeshRenderer, Transform[] bones)
        {
            BoneWeight[] boneWeights = new BoneWeight[skinnedMeshRenderer.sharedMesh.vertexCount];
            Vector3[] vertices = skinnedMeshRenderer.sharedMesh.vertices;

            // Пример распределения весов на основе расстояния до костей
            for (int i = 0; i < vertices.Length; i++)
            {
                // Найдем ближайшие кости для каждой вершины
                float minDistance1 = float.MaxValue;
                float minDistance2 = float.MaxValue;
                int boneIndex1 = 0;
                int boneIndex2 = 0;

                for (int j = 0; j < bones.Length; j++)
                {
                    float distance = Vector3.Distance(vertices[i], bones[j].position);

                    if (distance < minDistance1)
                    {
                        minDistance2 = minDistance1;
                        boneIndex2 = boneIndex1;

                        minDistance1 = distance;
                        boneIndex1 = j;
                    }
                    else if (distance < minDistance2)
                    {
                        minDistance2 = distance;
                        boneIndex2 = j;
                    }
                }

                // Пример распределения весов: чем ближе вершина к кости, тем больше её вес
                float totalDistance = minDistance1 + minDistance2;
                float weight1 = 1.0f - (minDistance1 / totalDistance);
                float weight2 = 1.0f - (minDistance2 / totalDistance);

                boneWeights[i].boneIndex0 = boneIndex1;
                boneWeights[i].weight0 = weight1;

                boneWeights[i].boneIndex1 = boneIndex2;
                boneWeights[i].weight1 = weight2;
            }

            // Применяем веса костей к мешу
            skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
        }
        private Matrix4x4 CreateBindPose(Transform bone, Transform root) => bone.worldToLocalMatrix * root.localToWorldMatrix; 
        private static void SetBoneWeightForFourNearesBone(SkinnedMeshRenderer skinnedMeshRenderer, Transform[] bones)
        {
            BoneWeight[] boneWeights = new BoneWeight[skinnedMeshRenderer.sharedMesh.vertexCount];
            Vector3[] vertices = skinnedMeshRenderer.sharedMesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                // Массивы для хранения ближайших костей и их расстояний
                float[] minDistances = { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue };
                int[] boneIndices = { -1, -1, -1, -1 };

                // Поиск 4 ближайших костей
                for (int j = 0; j < bones.Length; j++)
                {
                    float distance = Vector3.Distance(vertices[i], bones[j].position);

                    // Проверяем, если текущее расстояние меньше самого большого из 4 ближайших
                    for (int k = 0; k < 4; k++)
                    {
                        if (distance < minDistances[k])
                        {
                            // Сдвигаем текущие элементы вправо
                            for (int l = 3; l > k; l--)
                            {
                                minDistances[l] = minDistances[l - 1];
                                boneIndices[l] = boneIndices[l - 1];
                            }
                            // Обновляем расстояние и индекс кости
                            minDistances[k] = distance;
                            boneIndices[k] = j;
                            break;
                        }
                    }
                }

                // Теперь у нас есть 4 ближайших кости и их расстояния
                // Вычисляем веса для этих костей
                float totalDistance = minDistances[0] + minDistances[1] + minDistances[2] + minDistances[3];
                if (totalDistance == 0) totalDistance = 1; // Защита от деления на ноль

                boneWeights[i].boneIndex0 = boneIndices[0];
                boneWeights[i].weight0 = 1.0f - (minDistances[0] / totalDistance);

                boneWeights[i].boneIndex1 = boneIndices[1];
                boneWeights[i].weight1 = 1.0f - (minDistances[1] / totalDistance);

                boneWeights[i].boneIndex2 = boneIndices[2];
                boneWeights[i].weight2 = 1.0f - (minDistances[2] / totalDistance);

                boneWeights[i].boneIndex3 = boneIndices[3];
                boneWeights[i].weight3 = 1.0f - (minDistances[3] / totalDistance);
            }

            // Применяем веса костей к мешу
            skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
        } 
    }

    
}
