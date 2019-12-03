using System;
using System.Collections.Generic;
using UnityEngine;

namespace Slaggy.Utility
{
    [Serializable]
    public abstract class SerializableTypeBase
    {
        protected Type type;
        public virtual Type Type
        {
            get
            {
                if (type != null && _oldAssemblyQualifiedName == assemblyQualifiedName)
                    return type;

                type = Type.GetType(assemblyQualifiedName);
                _oldAssemblyQualifiedName = assemblyQualifiedName;
                return type;
            }
            set
            {
                type = value;
                _oldAssemblyQualifiedName = assemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }

        private string _oldAssemblyQualifiedName = string.Empty;
        [SerializeField] protected string assemblyQualifiedName = string.Empty;

        public virtual string AssemblyQualifiedName
        {
            get { return assemblyQualifiedName; }
            set { assemblyQualifiedName = value; }
        }

        protected SerializableTypeBase() { }
        protected SerializableTypeBase(string assemblyQualifiedName) : this(Type.GetType(assemblyQualifiedName)) { }

        protected SerializableTypeBase(Type type)
        {
            this.type = type;
            _oldAssemblyQualifiedName = assemblyQualifiedName = type.AssemblyQualifiedName;
        }

        public static implicit operator string(SerializableTypeBase serializableTypeBase)
        {
            return serializableTypeBase.assemblyQualifiedName;
        }
    }

    [Serializable]
    public abstract class ConstrainedSerializableTypeBase : SerializableTypeBase { }

    [Serializable]
    public abstract class ConstrainedSerializableType<TBaseConstraint> : ConstrainedSerializableTypeBase,
        IEquatable<ConstrainedSerializableType<TBaseConstraint>>, IEqualityComparer<ConstrainedSerializableType<TBaseConstraint>>
    {
        public override string AssemblyQualifiedName
        {
            get
            {
                return base.AssemblyQualifiedName;
            }
            set
            {
                Type newType = Type.GetType(value);
                if (newType != null && typeof(TBaseConstraint).IsAssignableFrom(newType))
                    base.AssemblyQualifiedName = value;
            }
        }

        public override Type Type
        {
            get
            {
                return base.Type;
            }
            set
            {
                if (value != null && typeof(TBaseConstraint).IsAssignableFrom(value))
                    base.Type = value;
            }
        }

        public bool Equals(ConstrainedSerializableType<TBaseConstraint> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && assemblyQualifiedName == other.assemblyQualifiedName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConstrainedSerializableType<TBaseConstraint>)obj);
        }

        public bool Equals(
            ConstrainedSerializableType<TBaseConstraint> x,
            ConstrainedSerializableType<TBaseConstraint> y)
        {
            return x != null && x.Equals(y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 353)
                       ^ (assemblyQualifiedName != null ? assemblyQualifiedName.GetHashCode() : 0);
            }
        }

        public int GetHashCode(ConstrainedSerializableType<TBaseConstraint> obj)
        {
            return obj.GetHashCode();
        }
    }

    [Serializable]
    public sealed class SerializableType : SerializableTypeBase, IEquatable<SerializableType>, IEqualityComparer<SerializableType>
    {
        public SerializableType(Type type) : base(type) { }
        public SerializableType(string assemblyQualifiedName) : base(assemblyQualifiedName) { }

        public bool Equals(SerializableType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && assemblyQualifiedName == other.assemblyQualifiedName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableType)obj);
        }

        public bool Equals(SerializableType x, SerializableType y)
        {
            return x != null && x.Equals(y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397)
                       ^ (assemblyQualifiedName != null ? assemblyQualifiedName.GetHashCode() : 0);
            }
        }

        public int GetHashCode(SerializableType obj)
        {
            return obj.GetHashCode();
        }
    }
}