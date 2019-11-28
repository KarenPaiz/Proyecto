using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librerias
{
    public class Metodos
    {
        public static byte[] EncryptionZigZag(byte[] TextoOriginal, int CantidadNiveles)
        {
            if (CantidadNiveles <= 1)
            {
                return TextoOriginal;
            }
            //Se crea un matriz vacia y se rellena con ~
            var MatrizCifrado = new byte[CantidadNiveles, TextoOriginal.Length];
            for (int i = 0; i < CantidadNiveles; i++)
            {
                for (int j = 0; j < TextoOriginal.Length; j++)
                {
                    MatrizCifrado[i, j] = 0;
                }
            }
            //Se hace el recorrido estilo zig zag
            var RecoridoBaja = false; var Fila = 0; var Columna = 0;
            for (int i = 0; i < TextoOriginal.Length; i++)
            {
                if (Fila == 0 || Fila == CantidadNiveles - 1)
                {
                    RecoridoBaja = !RecoridoBaja;
                }
                MatrizCifrado[Fila, Columna++] = TextoOriginal[i];
                if (RecoridoBaja)
                {
                    Fila++;
                }
                else
                {
                    Fila--;
                }
            }
            //Se crea el string encriptado
            var TextoEncriptado = new byte[TextoOriginal.Length];
            var h = 0;
            for (int i = 0; i < CantidadNiveles; i++)
            {
                for (int j = 0; j < TextoOriginal.Length; j++)
                {
                    if (MatrizCifrado[i, j] != 0)
                    {
                        TextoEncriptado[h] = MatrizCifrado[i, j];
                        h++;
                    }
                }
            }
            return TextoEncriptado;
        }
        public static byte[] DecryptZZ(byte[] TextoEncriptado, int CantidadNiveles)
        {
            if (CantidadNiveles <= 1)
            {
                return TextoEncriptado;
            }
            //Creacion y llenado de matriz
            var MatrizCifrada = new byte[CantidadNiveles, TextoEncriptado.Length];
            for (int i = 0; i < CantidadNiveles; i++)
            {
                for (int j = 0; j < TextoEncriptado.Length; j++)
                {
                    MatrizCifrada[i, j] = 0;
                }
            }
            //Hacer el recorrido en zig zag
            var HaciaAbajo = false;
            var Fila = 0; var Columna = 0;
            for (int i = 0; i < TextoEncriptado.Length; i++)
            {
                if (Fila == 0)
                {
                    HaciaAbajo = true;
                }
                if (Fila == CantidadNiveles - 1)
                {
                    HaciaAbajo = false;
                }
                MatrizCifrada[Fila, Columna++] = 1;
                if (HaciaAbajo)
                {
                    Fila++;
                }
                else
                {
                    Fila--;
                }
            }
            //Colocar los caracteres encriptados en la matriz
            var PosicionActual = 0;
            for (int i = 0; i < CantidadNiveles; i++)
            {
                for (int j = 0; j < TextoEncriptado.Length; j++)
                {
                    if (MatrizCifrada[i, j] == 1 && PosicionActual < TextoEncriptado.Length)
                    {
                        MatrizCifrada[i, j] = TextoEncriptado[PosicionActual++];
                    }
                }
            }
            //Desencriptar el texto
            var TextoDescifrado = new byte[TextoEncriptado.Length];
            var h = 0;
            Fila = 0; Columna = 0;
            for (int i = 0; i < TextoEncriptado.Length; i++)
            {
                if (Fila == 0)
                {
                    HaciaAbajo = true;
                }
                if (Fila == CantidadNiveles - 1)
                {
                    HaciaAbajo = false;
                }
                if (MatrizCifrada[Fila, Columna] != 1)
                {
                    TextoDescifrado[h] = (MatrizCifrada[Fila, Columna++]);
                    h++;
                }
                if (HaciaAbajo)
                {
                    Fila++;
                }
                else
                {
                    Fila--;
                }
            }
            return TextoDescifrado;
        }
        public static byte[] LZWCompress(byte[] TextoOriginal, string archivo)
        {
            var extension = "." + archivo.Split('.')[1];
            var charRegresa = new List<char>();
            var DiccionarioLZWCompresion = new Dictionary<string, int>();
            var listaCaracteresExistentes = new List<byte>();
            var listaCaracteresEscribir = new List<int>();
            var listaCaracteresBinario = new List<string>();
            foreach (var item in TextoOriginal)
            {
                if (!listaCaracteresExistentes.Contains(item))
                {
                    listaCaracteresExistentes.Add(item);
                }
            }
            listaCaracteresExistentes.Sort();
            foreach (var item in listaCaracteresExistentes)
            {
                var caractreres = Convert.ToChar(item);
                DiccionarioLZWCompresion.Add(caractreres.ToString(), DiccionarioLZWCompresion.Count + 1);
            }
            var TamanoDiccionario = Convert.ToString(DiccionarioLZWCompresion.LongCount()) + ".";
            for (int i = 0; i < TamanoDiccionario.Length; i++)
            {
                charRegresa.Add(Convert.ToChar(TamanoDiccionario[i]));
            }
            var CaracterActual = string.Empty;
            var Output = string.Empty;
            foreach (var item in TextoOriginal)
            {
                var CadenaAnalizada = CaracterActual + Convert.ToChar(item);
                if (DiccionarioLZWCompresion.ContainsKey(CadenaAnalizada))
                {
                    CaracterActual = CadenaAnalizada;
                }
                else
                {
                    listaCaracteresEscribir.Add(DiccionarioLZWCompresion[CaracterActual]);
                    DiccionarioLZWCompresion.Add(CadenaAnalizada, DiccionarioLZWCompresion.Count + 1);
                    CaracterActual = Convert.ToChar(item).ToString();
                }
            }
            listaCaracteresEscribir.Add(DiccionarioLZWCompresion[CaracterActual]);
            var TamanoTexto = Convert.ToString(DiccionarioLZWCompresion.LongCount()) + ".";
            for (int i = 0; i < TamanoTexto.Length; i++)
            {
                charRegresa.Add(Convert.ToChar(TamanoTexto[i]));
            }
            foreach (var item in listaCaracteresExistentes)
            {
                charRegresa.Add(Convert.ToChar(item));
            }
            charRegresa.Add('\u0002');
            var mayorIndice = listaCaracteresEscribir.Max();
            var bitsMayorIndice = (Convert.ToString(mayorIndice, 2)).Count();
            foreach (var item in bitsMayorIndice.ToString())
            {
                charRegresa.Add(Convert.ToChar(item));
            }
            foreach (var item in extension.ToCharArray())
            {
                charRegresa.Add(Convert.ToChar(item));
            }
            charRegresa.Add('\u0002');
            if (mayorIndice > 255)
            {
                foreach (var item in listaCaracteresEscribir)
                {
                    var indiceBinario = Convert.ToString(item, 2);
                    while (indiceBinario.Count() < bitsMayorIndice)
                    {
                        indiceBinario = "0" + indiceBinario;
                    }
                    listaCaracteresBinario.Add(indiceBinario);
                }
                var cadenaBits = string.Empty;
                foreach (var item in listaCaracteresBinario)
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        if (cadenaBits.Count() < 8)
                        {
                            cadenaBits += item[i];
                        }
                        else
                        {
                            var cadenaDecimal = Convert.ToInt64(cadenaBits, 2);
                            var cadenaEnByte = Convert.ToByte(cadenaDecimal);
                            charRegresa.Add(Convert.ToChar(cadenaEnByte));
                            cadenaBits = string.Empty;
                            cadenaBits += item[i];
                        }
                    }
                }
                if (cadenaBits.Length > 0)
                {
                    var cadenaRestante = Convert.ToInt64(cadenaBits, 2);
                    charRegresa.Add(Convert.ToChar(cadenaRestante));
                }
            }
            else
            {
                foreach (var item in listaCaracteresEscribir)
                {
                    charRegresa.Add(Convert.ToChar(Convert.ToInt32(item)));
                }
            }
            byte[] bytes = new byte[charRegresa.LongCount()];
            int cont = 0;
            foreach (var item in charRegresa)
            {
                bytes[cont] = Convert.ToByte(item);
                cont++;
            }
            return bytes;
        }
        public static byte[] LZWDecompress(byte[] TextoComprimido)
        {
            var bytesRegresa = new List<Byte>();
            var extensionArchivo = string.Empty;
            var DiccionarioText = string.Empty;
            var DiccionarioCaracteres = new Dictionary<int, string>();
            int contLec = 0;
            var CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
            contLec++;
            var CantidadCaracteresDiccionatrio = string.Empty;
            while (CaracterDiccionario != '.')
            {
                CantidadCaracteresDiccionatrio += CaracterDiccionario;
                CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
                contLec++;
            }
            var CantidadTexto = string.Empty;
            CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
            contLec++;
            while (CaracterDiccionario != '.')
            {
                CantidadTexto += CaracterDiccionario;
                CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
                contLec++;
            }
            CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
            var byteEscrito = (TextoComprimido[contLec]);
            while (DiccionarioCaracteres.Count != Convert.ToInt32(CantidadCaracteresDiccionatrio))
            {
                if (!DiccionarioCaracteres.ContainsValue(Convert.ToString(Convert.ToChar(byteEscrito))))
                {
                    DiccionarioCaracteres.Add(DiccionarioCaracteres.Count + 1, Convert.ToString(Convert.ToChar(byteEscrito)));
                }
                byteEscrito = (TextoComprimido[contLec]);
                contLec++;
            }
            CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
            contLec++;
            var TamanoBits = string.Empty;
            while (CaracterDiccionario != '.')
            {
                TamanoBits += CaracterDiccionario;
                CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
                contLec++;
            }
            CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
            contLec++;
            while (CaracterDiccionario != '\u0002')
            {
                extensionArchivo += CaracterDiccionario;
                CaracterDiccionario = Convert.ToChar(TextoComprimido[contLec]);
                contLec++;
            }
            extensionArchivo = "." + extensionArchivo;
            var byteAnalizado = string.Empty;
            var listaCaracteresComprimidos = new List<int>();
            while (contLec != TextoComprimido.Length && listaCaracteresComprimidos.Count < Convert.ToInt32(CantidadTexto))
            {
                var A = (TextoComprimido[contLec]);
                contLec++;
                var byteLeido = Convert.ToString(A, 2);
                while (byteLeido.Length < 8)
                {
                    byteLeido = "0" + byteLeido;
                }
                byteAnalizado += byteLeido;
                if (Convert.ToInt32(TamanoBits) > 8)
                {
                    if (byteAnalizado.Length >= Convert.ToInt32(TamanoBits))
                    {
                        var caracterComprimido = string.Empty;
                        for (int i = 0; i < Convert.ToInt32(TamanoBits); i++)
                        {
                            caracterComprimido += byteAnalizado[i];
                        }
                        listaCaracteresComprimidos.Add(Convert.ToInt32(caracterComprimido, 2));
                        byteAnalizado = byteAnalizado.Substring(Convert.ToInt32(TamanoBits));
                    }
                }
                else
                {
                    listaCaracteresComprimidos.Add(Convert.ToInt32(byteAnalizado, 2));
                    byteAnalizado = string.Empty;
                }
            }
            if (byteAnalizado.Length > 0)
            {
                listaCaracteresComprimidos[listaCaracteresComprimidos.Count - 1] = listaCaracteresComprimidos[listaCaracteresComprimidos.Count - 1] + Convert.ToInt32(byteAnalizado, 2);
            }
            var primerCaracter = DiccionarioCaracteres[listaCaracteresComprimidos[0]];
            listaCaracteresComprimidos.RemoveAt(0);
            var decompressed = new System.Text.StringBuilder(primerCaracter);
            foreach (var item in listaCaracteresComprimidos)
            {
                var cadenaAnalizada = string.Empty;
                if (DiccionarioCaracteres.ContainsKey(item))
                {
                    cadenaAnalizada = DiccionarioCaracteres[item];
                }
                else if (item == DiccionarioCaracteres.Count + 1)
                {
                    cadenaAnalizada = primerCaracter + primerCaracter[0];
                }
                decompressed.Append(cadenaAnalizada);
                DiccionarioCaracteres.Add(DiccionarioCaracteres.Count + 1, primerCaracter + cadenaAnalizada[0]);
                primerCaracter = cadenaAnalizada;
            }
            var texto = decompressed.ToString().ToCharArray();
            foreach (var item in texto)
            {
                bytesRegresa.Add(Convert.ToByte(item));
            }
            return bytesRegresa.ToArray();
        }

        /*public static BigInteger DiffieHelmannAlgorithm(int numberA, int numberB)
   {
       BigInteger numberG = 11;
       BigInteger numberP = 23;
       BigInteger numberFromA = BigInteger.ModPow(numberG, (numberA), numberP);
       BigInteger numberFromB = BigInteger.ModPow(numberG, (numberB), numberP);
       BigInteger SecretKeyFromA = BigInteger.ModPow(numberFromB, (numberA), numberP);
       BigInteger SecretKeyFromB = BigInteger.ModPow(numberFromA, (numberB), numberP);
       if (SecretKeyFromA == SecretKeyFromB)
       {
           return SecretKeyFromA;
       }
       return 0;
   }
   */
    }
}
