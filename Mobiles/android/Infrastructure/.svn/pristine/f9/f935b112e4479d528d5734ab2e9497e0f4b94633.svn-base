package wqfree.com;

import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;

public class UtilityFile {

	// public static void putPhoneNum(String path, String fileName, String
	// phoneNum) {
	// try {
	// File _file = new File(path, fileName);
	// if (!_file.exists())
	// _file.createNewFile();
	// if (_file.exists()) {
	// FileOutputStream _outStream = new FileOutputStream(_file);
	// PrintWriter _print = new PrintWriter(_outStream);
	// _print.write(phoneNum);
	// _print.flush();
	// _print.close();
	// }
	// } catch (Exception e) {
	// }
	// }
	//
	// public static String getPhoneNum(String path, String fileName) {
	// StringBuilder phone = new StringBuilder();
	// try {
	// File _file = new File(path, fileName);
	// if (_file.exists()) {
	// BufferedReader br = new BufferedReader(new FileReader(_file));
	// phone=br.readLine();
	// while((phone = br.readLine())!=null){
	//
	// }
	//
	// }
	// } catch (Exception e) {
	// }
	// return phone;
	// }

	public static byte[] readImg(String pdirPath, String fileName) {
		File imgFile = new File(pdirPath, fileName);
		try {
			if (imgFile.exists()) {
				FileInputStream inputStream = new FileInputStream(imgFile);
				return readStream(inputStream);
			}
		} catch (Exception e) {
		}
		return null;
	}

	public static byte[] readStream(InputStream inStream) {
		ByteArrayOutputStream outSteam = new ByteArrayOutputStream();
		try {
			byte[] buffer = new byte[1024];
			int len = -1;
			while ((len = inStream.read(buffer)) != -1) {
				outSteam.write(buffer, 0, len);
			}
			outSteam.close();
			inStream.close();
		} catch (Exception e) {
		}
		return outSteam.toByteArray();
	}

	public static String readFile(String pdirPath, String fileName) {
		StringBuilder res = new StringBuilder();
		try {
			File _file = new File(pdirPath, fileName);
			if (_file.exists()) {
				BufferedReader br = new BufferedReader(new FileReader(_file));
				String s = "";
				while ((s = br.readLine()) != null) {
					res.append(s).append("\n");
				}
			}
		} catch (Exception e) {
		}
		return res.toString();
	}

	public static void writeFile(Object content, String pdirPath,
			String fileName) {
		File dirFile = new File(pdirPath);

		if (!dirFile.exists()) {
			dirFile.mkdirs();
		}
		if (dirFile.exists()) {
			try {
				File file = new File(pdirPath, fileName);
				FileOutputStream out = new FileOutputStream(file);
				BufferedOutputStream outputStream = new BufferedOutputStream(
						out);
				if (content instanceof String) {
					outputStream.write(((String) content).getBytes());
				} else if (content instanceof byte[]) {
					byte[] buffer = (byte[]) content;
					outputStream.write(buffer);
				}
				outputStream.flush();
				out.close();
				outputStream.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}

}
