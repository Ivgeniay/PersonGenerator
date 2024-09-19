using AtomEngine.SystemFunc.Attributes;
using AtomEngine.Testing.Inspector;
using System.Collections.Generic;  
using AtomEngine.Meshes.Chains;
using AtomEngine.Meshes;
using System.Reflection; 
using System.Linq;
using UnityEngine;
using System;  

namespace AtomEngine.Skillets
{
    [RequireComponent(typeof(SkilletBuilder), typeof(MeshChainConnector))]
    public class MeshBuilder : MonoBehaviour, IPublicMethodsInspector
    {
        [SerializeField] private Material material; 
        public MeshChainConnector MeshChainConnector;
        public SkilletBuilder SkilletBuilder;

        [SerializeField] private List<BonesTransform_MeshChain_Connection_Model> _bonesTransformBindedField = new();
        [SerializeField] private List<Transform> meshPointsTransforms = new();

        public GameObject meshRanderRoot;

        public void GenerateMesh()
        { 
            if (!Validation()) return; 

            FirstStep_CreateMeshRenderRoot();
            SecondStep_Clone_Skillet_Hyerarhy_And_Attach_MeshChain();
            ThirdStep_MakeAtomarConnection();
            FourthStep_CombineMeshes();
            FifthStep_DestroyAtomarMeshes();
            SixthStep_Generate_SkinRenderer();
        }

        public bool Validation()
        {
            FillBones();
            if (!Validation_p())
            {
                Debug.LogError("Validation is not pass");
                return false;
            }
            return true;
        }

        public void FirstStep_CreateMeshRenderRoot() => meshRanderRoot = new GameObject($"{nameof(MeshBuilder)}Root");
        public void SecondStep_Clone_Skillet_Hyerarhy_And_Attach_MeshChain()
        {
            SkilletBuilder.transform.CloneHyerarhy(meshRanderRoot.transform, ref meshPointsTransforms);
            Attach_RendererRefferenceTransform();
            Attach_MeshChain_To_RenderObjects();
            meshRanderRoot.transform.SetParent(SkilletBuilder.transform);
        }
        public void ThirdStep_MakeAtomarConnection() => Make_Atomar_Connection(); 
        public void FourthStep_CombineMeshes() => CombineMesh(SkilletBuilder.transform);
        public void FifthStep_DestroyAtomarMeshes() => DestroyAtomarMeshes(meshRanderRoot);
        public void SixthStep_Generate_SkinRenderer() => GenerateSkinRenerer(this.transform);


        private void FillBones()
        {
            if (!SkilletBuilder) SkilletBuilder = GetComponent<SkilletBuilder>();
            meshPointsTransforms.Clear();
            _bonesTransformBindedField.Clear();

            var bonesTransformBindedField = SkilletBuilder.GetBoneTransforms();
            foreach(var Transform_FieldInfo_Pair in bonesTransformBindedField)
            {
                _bonesTransformBindedField.Add(new BonesTransform_MeshChain_Connection_Model()
                {
                    BoneTransform = Transform_FieldInfo_Pair.Item1,
                    Connection = Transform_FieldInfo_Pair.Item2.GetCustomAttribute<ConnectionAttribute>()
                });
            }
        }
        private bool Validation_p()
        {
            if (MeshChainConnector == null) MeshChainConnector = GetComponent<MeshChainConnector>();
            meshPointsTransforms.Clear();
            return true;
        }
        

        private void Attach_RendererRefferenceTransform()
        {
            foreach (var clone in meshPointsTransforms)
            {
                var refer = _bonesTransformBindedField.FirstOrDefault(x => x.BoneTransform.name == clone.name);
                if (refer != null) refer.RenderRefferenceTransform = clone;
            }
        }
        private void Attach_MeshChain_To_RenderObjects()
        {
            foreach(var transform in meshPointsTransforms)
            {
                var mChain = transform.gameObject.AddComponent<MeshChain>();
                mChain.Initialize(4, 0.05f);
                var refer = _bonesTransformBindedField.FirstOrDefault(x => x.RenderRefferenceTransform == transform);
                if (refer != null) refer.MeshChain = mChain;
            }
        } 
        private void Make_Atomar_Connection()
        {
            foreach(var model in _bonesTransformBindedField)
            {
                var connectionModel = _bonesTransformBindedField.FirstOrDefault(e => model.Connection.ConnectionFieldName == e.BoneTransform.name);

                if (connectionModel == default)
                {
                    Debug.Log($"No connection found for {model.BoneTransform.name}");
                    continue;
                }
                if (connectionModel.MeshChain == null)
                {
                    Debug.LogError($"Нет соединения для {model.BoneTransform.name} c {model.Connection.ConnectionFieldName}");
                    continue;
                }

                MeshRenderer meshRenderer = model.MeshChain.gameObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = model.MeshChain.gameObject.AddComponent<MeshFilter>();
                meshRenderer.material = material;
                MeshChainConnector.Connect(new List<MeshChain>() { model.MeshChain, connectionModel.MeshChain }, meshFilter);
            }

            //foreach (var (bone, meshChain, field) in bonesTransformMeshChainBindedField)
            //{
            //    ConnectionAttribute attribute = field.GetCustomAttribute<ConnectionAttribute>();
            //    if (attribute == null)
            //    {
            //        Debug.Log($"No ConnectionAttribute found for {field.Name}");
            //        continue;
            //    }
            //    var connectionTupple = bonesTransformMeshChainBindedField.FirstOrDefault(e => e.Item3.Name == attribute.ConnectionFieldName);
            //    if (connectionTupple == default)
            //    {
            //        Debug.Log($"No connection found for {field.Name}");
            //        continue;
            //    }

            //    MeshRenderer meshRenderer = meshChain.gameObject.AddComponent<MeshRenderer>();
            //    MeshFilter meshFilter = meshChain.gameObject.AddComponent<MeshFilter>();
            //    meshRenderer.material = material;
            //    MeshChainConnector.Connect(new List<MeshChain>() { meshChain, connectionTupple.Item2 }, meshFilter);
            //}
        }
        private void CombineMesh(Transform root)
        {
            IEnumerable<MeshFilter> meshes = root.GetComponentsInChildren<MeshFilter>()
                                                .Where(mf => mf.GetComponent<MeshChain>() != null);

            Mesh combinedMesh = MeshConnector.Combine(meshes.ToArray());

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

    [Serializable]
    public class BonesTransform_MeshChain_Connection_Model
    {
        [field: SerializeField] public Transform BoneTransform { get; set; }
        [field: SerializeField] public MeshChain MeshChain { get; set; }
        [field: SerializeField] public Transform RenderRefferenceTransform { get; set; }
        [field: SerializeField] public ConnectionAttribute Connection { get; set; }
    }
}
