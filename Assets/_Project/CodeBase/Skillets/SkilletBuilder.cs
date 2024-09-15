using System.Collections.Generic;
using SystemFunc.Attributes;
using SystemFunc.Transforms;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Text;
using System;

namespace Skillets
{
    [RequireComponent(typeof(MeshBuilder))]
    public class SkilletBuilder : MonoBehaviour
    {
        [SerializeField] 
        private MixamoMensConfigurator mensMixamoConfigurator;

        [SerializeField] 
        public Transform RootPerson;
        
        [Space(10)]
        [SerializeField]
        [Connection("RootPerson", BodyChapterType = BodyChapterType.Body)]                              
        public Transform Hip;

        [SerializeField]
        [Connection("Hip", BodyChapterType = BodyChapterType.LLeg)]                                     
        public Transform LeftUpLeg;
        [SerializeField]
        [Connection("LeftUpLeg", BodyChapterType = BodyChapterType.LLeg)]                               
        public Transform LeftLeg;
        [SerializeField]
        [Connection("LeftLeg", BodyChapterType = BodyChapterType.LLeg)]                                 
        public Transform LeftFoot;
        [SerializeField]
        [Connection("LeftFoot", BodyChapterType = BodyChapterType.LLeg)]                                
        public Transform LeftToeBase;
        [SerializeField]
        [Connection("LeftToeBase", BodyChapterType = BodyChapterType.LLeg, LastPoint = true)]           
        public Transform LeftToe_End;

        [SerializeField]
        [Connection("Hip", BodyChapterType = BodyChapterType.RLeg)]
        public Transform RightUpLeg;
        [SerializeField]
        [Connection("RightUpLeg", BodyChapterType = BodyChapterType.RLeg)]
        public Transform RightLeg;
        [SerializeField]
        [Connection("RightLeg", BodyChapterType = BodyChapterType.RLeg)]
        public Transform RightFoot;
        [SerializeField]
        [Connection("RightFoot", BodyChapterType = BodyChapterType.RLeg)]
        public Transform RightToeBase;
        [SerializeField]
        [Connection("RightToeBase", BodyChapterType = BodyChapterType.RLeg, LastPoint = true)]
        public Transform RightToe_End;

        [SerializeField]
        [Connection("Hip", BodyChapterType = BodyChapterType.Body)]
        public Transform Spine;
        [SerializeField]
        [Connection("Spine", BodyChapterType = BodyChapterType.Body)]
        public Transform Spine1;
        [SerializeField]
        [Connection("Spine1", BodyChapterType = BodyChapterType.Body)]
        public Transform Spine2;

        [SerializeField]
        [Connection("Spine2", BodyChapterType = BodyChapterType.LHand)]
        public Transform LeftShoulder; 
        [SerializeField]
        [Connection("LeftShoulder", BodyChapterType = BodyChapterType.LHand)]                            
        public Transform LeftArm;
        [SerializeField]
        [Connection("LeftArm", BodyChapterType = BodyChapterType.LHand)]                                 
        public Transform LeftForeArm;
        [SerializeField]
        [Connection("LeftForeArm", BodyChapterType = BodyChapterType.LHand)]                             
        public Transform LeftHand;
        [SerializeField]
        [Connection("LeftHand", BodyChapterType = BodyChapterType.LHand)]                                
        public Transform LeftHandIndex1;
        [SerializeField]
        [Connection("LeftHandIndex1", BodyChapterType = BodyChapterType.LHand)]                          
        public Transform LeftHandIndex2;
        [SerializeField]
        [Connection("LeftHandIndex2", BodyChapterType = BodyChapterType.LHand)]                          
        public Transform LeftHandIndex3;
        [SerializeField]
        [Connection("LeftHandIndex3", BodyChapterType = BodyChapterType.LHand, LastPoint = true)]        
        public Transform LeftHandIndex4;

        [SerializeField]
        [Connection("Spine2", BodyChapterType = BodyChapterType.Head)]                                  
        public Transform Neck;
        [SerializeField]
        [Connection("Neck", BodyChapterType = BodyChapterType.Head)]                                    
        public Transform Head;
        [SerializeField]
        [Connection("Neck", BodyChapterType = BodyChapterType.Head, LastPoint = true)]                  
        public Transform HeadTop_End; 

        [SerializeField]
        [Connection("Spine2", BodyChapterType = BodyChapterType.RHand)]                                  
        public Transform RightShoulder;
        [SerializeField]
        [Connection("RightShoulder", BodyChapterType = BodyChapterType.RHand)]                           
        public Transform RightArm;
        [SerializeField]
        [Connection("RightArm", BodyChapterType = BodyChapterType.RHand)]                                
        public Transform RightForeArm;
        [SerializeField]
        [Connection("RightForeArm", BodyChapterType = BodyChapterType.RHand)]                            
        public Transform RightHand;
        [SerializeField]
        [Connection("RightHand", BodyChapterType = BodyChapterType.RHand)]                               
        public Transform RightHandIndex1;
        [SerializeField]
        [Connection("RightHandIndex1", BodyChapterType = BodyChapterType.RHand)]                         
        public Transform RightHandIndex2;
        [SerializeField]
        [Connection("RightHandIndex2", BodyChapterType = BodyChapterType.RHand)]                         
        public Transform RightHandIndex3;
        [SerializeField]
        [Connection("RightHandIndex3", BodyChapterType = BodyChapterType.RHand, LastPoint = true)]       
        public Transform RightHandIndex4;

        public void AutoConfigure()
        {
            if (RootPerson == null)
            {
                Debug.LogError("RootPerson is not set");
                return; 
            }
            IEnumerable<Transform> childs = RootPerson.GetAllTransformChilds();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Transform))
                { 
                    var matchingChild = childs.FirstOrDefault(child => child.name.Contains(field.Name, StringComparison.OrdinalIgnoreCase));
                    if (matchingChild != null)
                        field.SetValue(this, matchingChild);
                }
            }
        }  
        public Transform GetRoot()
        {
            if (RootPerson == null) Debug.LogError("Root is null"); 
            return RootPerson;
        } 
        public IEnumerable<(Transform, FieldInfo)> GetBoneTransforms()
        {
            var fields = GetType()
                .GetFields(BindingFlags.Public | 
                            BindingFlags.NonPublic | 
                            BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.Name == "RootPerson") continue;
                if (field.FieldType == typeof(Transform) && field.GetValue(this) != null)
                    yield return ((Transform)field.GetValue(this), field);
            }
        } 
        public void CreateNewSkilet()
        {
            Dictionary<string, FieldInfo> fieldMap = new Dictionary<string, FieldInfo>(); 
            if (RootPerson == null)
            {
                RootPerson = this.transform;
            }
             
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields) fieldMap.Add(field.Name, field); 
            foreach (var field in fields)
            {
                ConnectionAttribute connection = field.GetCustomAttribute<ConnectionAttribute>();
                if (connection != null) CreateObjectChain(field.Name, fieldMap);
            }
        }
        public void Clear()
        {
            IEnumerable<Transform> childs = RootPerson.GetAllTransformChilds();
            var fields = GetType().GetFields(BindingFlags.Public |
                                         BindingFlags.NonPublic |
                                         BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Transform))
                    field.SetValue(this, null);
            }

            //var str = configurator.SelfWriter(GetBoneTransforms());
            //Debug.Log(str);
        } 
        private void CreateObjectChain(string fieldName, Dictionary<string, FieldInfo> fieldMap)
        {
            var field = fieldMap[fieldName];
            Transform currentTransform = (Transform)field.GetValue(this);
            if (currentTransform != null)
            {
                return;
            }

            ConnectionAttribute connection = field.GetCustomAttribute<ConnectionAttribute>();
            if (connection != null)
            {
                if (fieldMap.ContainsKey(connection.ConnectionFieldName))
                {
                    CreateObjectChain(connection.ConnectionFieldName, fieldMap);
                    Transform parentTransform = (Transform)fieldMap[connection.ConnectionFieldName].GetValue(this);

                    GameObject newObject = new GameObject(fieldName);
                    Transform newTransform = newObject.transform;

                    ConfigureBone fieldData = mensMixamoConfigurator.GetVieldInfo(fieldName); 
                    newTransform.SetParent(parentTransform);
                    newTransform.localPosition = fieldData.LocalPositon;
                    newTransform.localRotation = fieldData.LocalRotation; 

                    field.SetValue(this, newTransform); 
                }
                else
                {
                    Debug.LogError($"Не найдено родительское поле {connection.ConnectionFieldName} для {fieldName}");
                }
            }
        }
    }

    [Serializable]
    public class MixamoMensConfigurator
    {
        [SerializeField]
        public ConfigureBone Hip = new ConfigureBone()
        {
            BoneName = "Hip",
            LocalPositon = new Vector3(3.301595E-07f, 0.9979187f, 5.162134E-07f),
            LocalRotation = new Quaternion(7.537291E-11f, 1.753127E-06f, -4.388351E-05f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftUpLeg = new ConfigureBone()
        {
            BoneName = "LeftUpLeg",
            LocalPositon = new Vector3(-0.09123874f, -0.06657188f, -0.0005540311f),
            LocalRotation = new Quaternion(-2.029215E-05f, -0.006339882f, 0.9999755f, 0.002969751f)
        };

        [SerializeField]
        public ConfigureBone LeftLeg = new ConfigureBone()
        {
            BoneName = "LeftLeg",
            LocalPositon = new Vector3(4.796163E-16f, 0.4059944f, 1.170175E-15f),
            LocalRotation = new Quaternion(-0.01813409f, -0.0001825504f, 0.005920147f, 0.999818f)
        };

        [SerializeField]
        public ConfigureBone LeftFoot = new ConfigureBone()
        {
            BoneName = "LeftFoot",
            LocalPositon = new Vector3(8.881784E-17f, 0.4209903f, 2.087219E-16f),
            LocalRotation = new Quaternion(0.5405577f, -0.01436606f, -0.02495087f, 0.8408142f)
        };

        [SerializeField]
        public ConfigureBone LeftToeBase = new ConfigureBone()
        {
            BoneName = "LeftToeBase",
            LocalPositon = new Vector3(-2.454674E-17f, 0.1572156f, -1.552881E-16f),
            LocalRotation = new Quaternion(0.2276921f, 0.03252147f, 0.01536559f, 0.9730687f)
        };

        [SerializeField]
        public ConfigureBone LeftToe_End = new ConfigureBone()
        {
            BoneName = "LeftToe_End",
            LocalPositon = new Vector3(-1.776357E-17f, 0.09999999f, 4.890747E-11f),
            LocalRotation = new Quaternion(0f, 0f, 0f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightUpLeg = new ConfigureBone()
        {
            BoneName = "RightUpLeg",
            LocalPositon = new Vector3(0.09125032f, -0.06655601f, -0.0005535274f),
            LocalRotation = new Quaternion(1.796428E-05f, -0.006333346f, 0.9999753f, -0.003057252f)
        };

        [SerializeField]
        public ConfigureBone RightLeg = new ConfigureBone()
        {
            BoneName = "RightLeg",
            LocalPositon = new Vector3(-5.32907E-17f, 0.4059944f, 2.664535E-16f),
            LocalRotation = new Quaternion(-0.01814707f, 0.0001824914f, -0.00592015f, 0.9998178f)
        };

        [SerializeField]
        public ConfigureBone RightFoot = new ConfigureBone()
        {
            BoneName = "RightFoot",
            LocalPositon = new Vector3(2.131628E-16f, 0.4209903f, -6.261658E-16f),
            LocalRotation = new Quaternion(0.5405632f, 0.0143663f, 0.02495166f, 0.8408106f)
        };

        [SerializeField]
        public ConfigureBone RightToeBase = new ConfigureBone()
        {
            BoneName = "RightToeBase",
            LocalPositon = new Vector3(-2.369483E-17f, 0.1572156f, 3.455569E-17f),
            LocalRotation = new Quaternion(0.2277087f, -0.03211961f, -0.01525791f, 0.9730799f)
        };

        [SerializeField]
        public ConfigureBone RightToe_End = new ConfigureBone()
        {
            BoneName = "RightToe_End",
            LocalPositon = new Vector3(-3.188058E-11f, 0.09999999f, 5.687534E-11f),
            LocalRotation = new Quaternion(0f, 0f, 0f, 1f)
        };

        [SerializeField]
        public ConfigureBone Spine = new ConfigureBone()
        {
            BoneName = "Spine",
            LocalPositon = new Vector3(-8.597855E-06f, 0.09923462f, -0.01227335f),
            LocalRotation = new Quaternion(-0.06073022f, -2.298403E-08f, -5.870007E-06f, 0.9981542f)
        };

        [SerializeField]
        public ConfigureBone Spine1 = new ConfigureBone()
        {
            BoneName = "Spine1",
            LocalPositon = new Vector3(-6.920259E-21f, 0.1173198f, -1.998401E-17f),
            LocalRotation = new Quaternion(0.0001960955f, 4.006073E-13f, 9.316072E-06f, 1f)
        };

        [SerializeField]
        public ConfigureBone Spine2 = new ConfigureBone()
        {
            BoneName = "Spine2",
            LocalPositon = new Vector3(-1.931492E-13f, 0.1345884f, 6.261658E-15f),
            LocalRotation = new Quaternion(0.05771172f, -2.195823E-06f, -3.285017E-06f, 0.9983333f)
        };

        [SerializeField]
        public ConfigureBone LeftShoulder = new ConfigureBone()
        {
            BoneName = "LeftShoulder",
            LocalPositon = new Vector3(-0.06105824f, 0.09106292f, 0.007570625f),
            LocalRotation = new Quaternion(0.4538693f, -0.5448208f, 0.5511668f, 0.4397592f)
        };

        [SerializeField]
        public ConfigureBone LeftArm = new ConfigureBone()
        {
            BoneName = "LeftArm",
            LocalPositon = new Vector3(-4.425508E-17f, 0.1292229f, 5.131805E-17f),
            LocalRotation = new Quaternion(-0.01047078f, 0.001064855f, -0.1011494f, 0.9948156f)
        };

        [SerializeField]
        public ConfigureBone LeftForeArm = new ConfigureBone()
        {
            BoneName = "LeftForeArm",
            LocalPositon = new Vector3(-3.059411E-11f, 0.2740468f, -1.516566E-16f),
            LocalRotation = new Quaternion(-2.980232E-08f, 7.450581E-08f, 2.220446E-15f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftHand = new ConfigureBone()
        {
            BoneName = "LeftHand",
            LocalPositon = new Vector3(1.282042E-07f, 0.2761446f, 2.336106E-09f),
            LocalRotation = new Quaternion(5.960464E-08f, -1.788139E-07f, -1.192093E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftHandIndex1 = new ConfigureBone()
        {
            BoneName = "LeftHandIndex1",
            LocalPositon = new Vector3(0.02822044f, 0.1226662f, 0.002318252f),
            LocalRotation = new Quaternion(-2.56265E-07f, -8.632578E-08f, 3.056184E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftHandIndex2 = new ConfigureBone()
        {
            BoneName = "LeftHandIndex2",
            LocalPositon = new Vector3(1.476015E-17f, 0.03891968f, -4.035612E-18f),
            LocalRotation = new Quaternion(-1.192093E-07f, 5.960464E-08f, 1.652723E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftHandIndex3 = new ConfigureBone()
        {
            BoneName = "LeftHandIndex3",
            LocalPositon = new Vector3(-4.071323E-13f, 0.03415161f, -8.802038E-16f),
            LocalRotation = new Quaternion(-8.940695E-08f, -1.490111E-08f, -1.192092E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone LeftHandIndex4 = new ConfigureBone()
        {
            BoneName = "LeftHandIndex4",
            LocalPositon = new Vector3(-5.854722E-09f, 0.03077988f, 1.627003E-16f),
            LocalRotation = new Quaternion(1.039359E-06f, -0.004001603f, -2.277958E-05f, 0.999992f)
        };

        [SerializeField]
        public ConfigureBone Neck = new ConfigureBone()
        {
            BoneName = "Neck",
            LocalPositon = new Vector3(-2.548123E-07f, 0.1502776f, 0.008779068f),
            LocalRotation = new Quaternion(0.002827706f, 1.136864E-13f, 3.214719E-16f, 0.999996f)
        };

        [SerializeField]
        public ConfigureBone Head = new ConfigureBone()
        {
            BoneName = "Head",
            LocalPositon = new Vector3(-2.562549E-08f, 0.1032184f, 0.03142429f),
            LocalRotation = new Quaternion(5.654555E-27f, -1.058791E-22f, -2.615043E-21f, 1f)
        };

        [SerializeField]
        public ConfigureBone HeadTop_End = new ConfigureBone()
        {
            BoneName = "HeadTop_End",
            LocalPositon = new Vector3(-1.545159E-06f, 0.1847467f, 0.06636399f),
            LocalRotation = new Quaternion(6.938894E-18f, 0f, 0f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightShoulder = new ConfigureBone()
        {
            BoneName = "RightShoulder",
            LocalPositon = new Vector3(0.06105696f, 0.09106383f, 0.007570756f),
            LocalRotation = new Quaternion(-0.4538035f, -0.5448757f, 0.5511121f, -0.4398276f)
        };

        [SerializeField]
        public ConfigureBone RightArm = new ConfigureBone()
        {
            BoneName = "RightArm",
            LocalPositon = new Vector3(-3.913536E-17f, 0.1292229f, -8.437695E-17f),
            LocalRotation = new Quaternion(-0.01043548f, -0.001061274f, 0.1011499f, 0.9948159f)
        };

        [SerializeField]
        public ConfigureBone RightForeArm = new ConfigureBone()
        {
            BoneName = "RightForeArm",
            LocalPositon = new Vector3(-8.327426E-11f, 0.2740468f, -1.736068E-16f),
            LocalRotation = new Quaternion(-2.980232E-08f, -7.450581E-08f, -2.220446E-15f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightHand = new ConfigureBone()
        {
            BoneName = "RightHand",
            LocalPositon = new Vector3(-1.284284E-07f, 0.2761446f, 1.54924E-07f),
            LocalRotation = new Quaternion(5.960464E-08f, 1.788139E-07f, 1.192093E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightHandIndex1 = new ConfigureBone()
        {
            BoneName = "RightHandIndex1",
            LocalPositon = new Vector3(-0.02822042f, 0.1226662f, 0.002318252f),
            LocalRotation = new Quaternion(-2.860701E-07f, 2.212001E-08f, 4.402059E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightHandIndex2 = new ConfigureBone()
        {
            BoneName = "RightHandIndex2",
            LocalPositon = new Vector3(-2.653051E-18f, 0.03891968f, -3.752315E-17f),
            LocalRotation = new Quaternion(-8.940695E-08f, 1.490115E-08f, 2.277623E-08f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightHandIndex3 = new ConfigureBone()
        {
            BoneName = "RightHandIndex3",
            LocalPositon = new Vector3(5.167483E-14f, 0.03415161f, 9.823766E-16f),
            LocalRotation = new Quaternion(-8.940695E-08f, 1.490111E-08f, 1.192092E-07f, 1f)
        };

        [SerializeField]
        public ConfigureBone RightHandIndex4 = new ConfigureBone()
        {
            BoneName = "RightHandIndex4",
            LocalPositon = new Vector3(1.277157E-08f, 0.03077988f, 7.899497E-18f),
            LocalRotation = new Quaternion(-7.290382E-05f, 0.006709308f, -0.001374533f, 0.9999766f)
        };

        public ConfigureBone GetVieldInfo(string fieldName)
        {
            if (fieldName.ToLower().Contains("leftupleg"))
            {

            }
            if (this.GetType().GetField(fieldName).GetValue(this) is ConfigureBone configureBone)
            {
                return configureBone;
            }
            return new ConfigureBone();
        }
        public string SelfWriter(IEnumerable<Transform> bones)
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (var bone in bones)
            {
                string lposx = bone.localPosition.x.ToString().Replace(",", ".").Replace(" ", "");
                string lposy = bone.localPosition.y.ToString().Replace(",", ".").Replace(" ", "");
                string lposz = bone.localPosition.z.ToString().Replace(",", ".").Replace(" ", "");
                string lRotx = bone.localRotation.x.ToString().Replace(",", ".").Replace(" ", "");
                string lRoty = bone.localRotation.y.ToString().Replace(",", ".").Replace(" ", "");
                string lRotz = bone.localRotation.z.ToString().Replace(",", ".").Replace(" ", "");
                string lRotw = bone.localRotation.w.ToString().Replace(",", ".").Replace(" ", "");

                sb.AppendLine($"        [SerializeField] public ConfigureBone {bone.name} = new ConfigureBone()");
                sb.AppendLine("        {");
                sb.AppendLine($"            BoneName = \"{bone.name}\",");
                sb.AppendLine($"            LocalPositon = new Vector3({lposx}f, {lposy}f, {lposz}f),");
                sb.AppendLine($"            LocalRotation = new Quaternion({lRotx}f, {lRoty}f, {lRotz}f, {lRotw}f)");
                sb.AppendLine("        };");
                sb.AppendLine("");
            } 
            return sb.ToString();
        }
    }

    [Serializable]
    public struct ConfigureBone
    {
        [SerializeField] public string BoneName;
        [SerializeField] public Vector3 LocalPositon;
        [SerializeField] public Quaternion LocalRotation;
    }
}
