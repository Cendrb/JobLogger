using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface.Helpers
{
    class TracTypeConverter<TSource, TTarget>
    {
        private Func<TSource, TTarget> convertToTarget;
        private Func<TTarget, TSource> convertToSource;

        public TracTypeConverter(Func<TSource, TTarget> convertToTarget, Func<TTarget, TSource> convertToSource)
        {
            this.convertToTarget = convertToTarget;
            this.convertToSource = convertToSource;
        }

        public TTarget ConvertToTarget(TSource source)
        {
            if (this.convertToTarget == null)
            {
                throw new NotImplementedException(string.Format("Conversion from source type {0} to target type {1} is not implemented", typeof(TSource).AssemblyQualifiedName, typeof(TTarget).AssemblyQualifiedName));
            }

            return this.convertToTarget(source);
        }

        public TSource ConvertToSource(TTarget target)
        {
            if (this.convertToSource == null)
            {
                throw new NotImplementedException(string.Format("Conversion from target type {0} to source type {1} is not implemented", typeof(TTarget).AssemblyQualifiedName, typeof(TSource).AssemblyQualifiedName));
            }

            return this.convertToSource(target);
        }
    }
}
