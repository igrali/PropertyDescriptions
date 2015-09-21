using System.Collections.Generic;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using System;

namespace PropertyDescriptions
{
    public class EffectViewModel
    {
        public BlurEffect blur { get; set; }

        public EffectViewModel()
        {
            this.blur = new BlurEffect();

            DetectPropertyDescriptions();
        }

        private void DetectPropertyDescriptions()
        {
            IPropertyDescriptions blurPropertyDescriptions = this.blur as IPropertyDescriptions;

            if (blurPropertyDescriptions != null)
            {
                var propertyDescription = blurPropertyDescriptions.PropertyDescriptions[nameof(this.blur.KernelSize)];

                this.DefaultValue = (int) propertyDescription.DefaultValue;
                this.MinValue = (int) propertyDescription.MinValue;
                this.MaxValue = (int) propertyDescription.MaxValue;
            }
        }

        public int DefaultValue { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
    }
}
